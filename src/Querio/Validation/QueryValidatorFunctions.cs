using System.Collections.Generic;
using System.Linq;

namespace Querio;

/// <summary>
/// The half of validation that deals with values: conditions, and the function calls that may stand
/// wherever a field does.
/// </summary>
public static partial class QueryValidator
{
    private sealed partial class Context
    {
        /// <summary>
        /// Resolves what a value expression yields, reporting anything wrong on the way. A field and
        /// a call are alternatives - naming both leaves the value ambiguous.
        /// </summary>
        private QueryFieldType? ValueType(QueryFieldRef? field, QueryFunctionCall? call, string path)
        {
            if (field is not null && call is not null)
            {
                Add(QueryErrorCode.AmbiguousValueSource,
                    "Both a field and a function call were given where only one value belongs.", path);
                return null;
            }
            if (field is not null) return ResolveField(field, path)?.Type;
            if (call is not null) return ValidateCall(call, QueryFunctionKind.Value, path)?.ReturnType;
            return null;
        }

        /// <summary>
        /// Checks a call against what the schema declares: that the function exists, that it is the
        /// kind this position needs, that the arity fits, and that every argument reads correctly.
        /// </summary>
        private QueryFunction? ValidateCall(QueryFunctionCall call, QueryFunctionKind expected, string path)
        {
            if (string.IsNullOrEmpty(call.Function))
            {
                Add(QueryErrorCode.UnknownFunction, "The call names no function.", path);
                return null;
            }

            var function = schema.FindFunction(call.Function);
            if (function is null)
            {
                Add(QueryErrorCode.UnknownFunction, $"The schema declares no function '{call.Function}'.", path);
                return null;
            }
            if (function.Kind != expected)
            {
                Add(QueryErrorCode.FunctionKindMismatch,
                    expected == QueryFunctionKind.Value
                        ? $"'{function.Key}' yields rows, so it cannot stand where a value belongs."
                        : $"'{function.Key}' yields a value, so it cannot stand where rows belong.",
                    path);
                return null;
            }

            var required = function.RequiredParameterCount;
            var supplied = call.Arguments.Count;
            if (supplied < required || supplied > function.Parameters.Count)
            {
                Add(QueryErrorCode.FunctionArgumentCount,
                    $"'{function.Key}' takes {Arity(required, function.Parameters.Count)}, but {supplied} were supplied.",
                    path);
            }

            for (var i = 0; i < supplied; i++)
            {
                var declared = i < function.Parameters.Count ? function.Parameters[i].Type : (QueryFieldType?)null;
                ValidateArgument(call.Arguments[i], declared, $"{path}.Arguments[{i}]");
            }
            return function;
        }

        private static string Arity(int required, int total)
            => required == total
                ? $"{total} argument(s)"
                : $"between {required} and {total} arguments";

        private void ValidateArgument(QueryOperand operand, QueryFieldType? declared, string path)
        {
            switch (operand.Kind)
            {
                case QueryOperandKind.Field:
                    if (operand.Field is null) Add(QueryErrorCode.MissingOperand, "The argument names no field.", path);
                    else ResolveField(operand.Field, path);
                    break;

                case QueryOperandKind.Function:
                    if (operand.Call is null) Add(QueryErrorCode.MissingOperand, "The argument names no function.", path);
                    else ValidateCall(operand.Call, QueryFunctionKind.Value, path);
                    break;

                case QueryOperandKind.List:
                    Add(QueryErrorCode.FunctionArgumentInvalid,
                        "A set of values cannot be passed as a single argument.", path);
                    break;

                case QueryOperandKind.Relative:
                    if (operand.Relative is null)
                    {
                        Add(QueryErrorCode.MissingOperand, "The argument holds no time offset.", path);
                    }
                    else if (declared is not null && declared != QueryFieldType.DateTime)
                    {
                        Add(QueryErrorCode.RelativeValueNotApplicable,
                            $"The parameter takes {declared}, so a time offset does not fit it.", path);
                    }
                    break;

                default:
                    if (declared is not null && operand.Value is not null
                        && !QueryValue.TryParse(operand.Value, declared.Value, out _))
                    {
                        Add(QueryErrorCode.FunctionArgumentInvalid,
                            $"'{operand.Value}' does not read as {declared}.", path);
                    }
                    break;
            }
        }

        private void ValidateFilter(QueryFilterGroup? group, string path, bool allowSelectTarget)
        {
            if (group is null) return;
            for (var i = 0; i < group.Conditions.Count; i++)
            {
                ValidateCondition(group.Conditions[i], $"{path}.Conditions[{i}]", allowSelectTarget);
            }
            for (var i = 0; i < group.Groups.Count; i++)
            {
                ValidateFilter(group.Groups[i], $"{path}.Groups[{i}]", allowSelectTarget);
            }
        }

        private void ValidateCondition(QueryCondition condition, string path, bool allowSelectTarget)
        {
            var targets = 0;
            if (condition.Field is not null) targets++;
            if (condition.Call is not null) targets++;
            if (!string.IsNullOrEmpty(condition.Select)) targets++;

            if (targets > 1)
            {
                Add(QueryErrorCode.AmbiguousConditionTarget,
                    "The condition names more than one thing to test.", path);
                return;
            }
            if (targets == 0)
            {
                Add(QueryErrorCode.MissingConditionTarget,
                    "The condition names neither a field, a function call nor a select output alias.", path);
                return;
            }

            QueryFieldType? type;
            if (!string.IsNullOrEmpty(condition.Select))
            {
                if (!allowSelectTarget)
                {
                    Add(QueryErrorCode.SelectConditionOutsideHaving,
                        $"'{condition.Select}' is a computed aggregate, which does not exist yet where this clause is applied. Move the condition to Having.",
                        path);
                }
                else if (!_outputAliases.Contains(condition.Select!))
                {
                    Add(QueryErrorCode.UnknownSelectAlias,
                        $"Nothing in the query is selected as '{condition.Select}'.", path);
                }
                // An aggregate is a number unless something says otherwise, which is enough to read
                // the value it is compared against.
                type = QueryFieldType.Number;
            }
            else
            {
                type = ValueType(condition.Field, condition.Call, path);
                var field = condition.Field is null ? null : FindField(condition.Field);
                if (field is { Filterable: false })
                {
                    Add(QueryErrorCode.FieldNotFilterable,
                        $"The schema marks '{condition.Field}' as not filterable.", path);
                }
                else if (type is not null)
                {
                    var allowed = field?.AllowedOperators ?? QueryDefaults.OperatorsFor(type.Value);
                    if (!allowed.Contains(condition.Operator))
                    {
                        Add(QueryErrorCode.OperatorNotAllowed,
                            $"The schema does not permit the {condition.Operator} operator here.", path);
                    }
                }
            }

            ValidateOperandCount(condition, path);
            ValidateOperand(condition.Value, $"{path}.Value", type);
            ValidateOperand(condition.Value2, $"{path}.Value2", type);
        }

        private void ValidateOperandCount(QueryCondition condition, string path)
        {
            if (QueryDefaults.TakesNoValue(condition.Operator))
            {
                if (condition.Value is not null || condition.Value2 is not null)
                {
                    Add(QueryErrorCode.UnexpectedOperand,
                        $"The {condition.Operator} operator takes no operand.", path);
                }
                return;
            }
            if (QueryDefaults.TakesTwoValues(condition.Operator))
            {
                if (condition.Value is null || condition.Value2 is null)
                {
                    Add(QueryErrorCode.MissingOperand,
                        $"The {condition.Operator} operator needs both a lower and an upper bound.", path);
                }
                return;
            }
            if (QueryDefaults.TakesValueList(condition.Operator))
            {
                if (condition.Value is null || condition.Value.Kind != QueryOperandKind.List)
                {
                    Add(QueryErrorCode.MissingOperand,
                        $"The {condition.Operator} operator needs a set of values.", path);
                }
                return;
            }
            if (condition.Value is null)
            {
                Add(QueryErrorCode.MissingOperand,
                    $"The {condition.Operator} operator needs a value to compare against.", path);
            }
        }

        private void ValidateOperand(QueryOperand? operand, string path, QueryFieldType? against)
        {
            if (operand is null) return;
            switch (operand.Kind)
            {
                case QueryOperandKind.Field:
                    if (operand.Field is null) Add(QueryErrorCode.MissingOperand, "The operand names no field.", path);
                    else ResolveField(operand.Field, path);
                    break;

                case QueryOperandKind.Function:
                    if (operand.Call is null) Add(QueryErrorCode.MissingOperand, "The operand names no function.", path);
                    else ValidateCall(operand.Call, QueryFunctionKind.Value, path);
                    break;

                case QueryOperandKind.List:
                    if (operand.Values is null || operand.Values.Count == 0)
                    {
                        Add(QueryErrorCode.MissingOperand, "The operand holds an empty set of values.", path);
                    }
                    break;

                case QueryOperandKind.Relative:
                    if (operand.Relative is null)
                    {
                        Add(QueryErrorCode.MissingOperand, "The operand holds no time offset.", path);
                    }
                    else if (against is not null && against != QueryFieldType.DateTime)
                    {
                        Add(QueryErrorCode.RelativeValueNotApplicable,
                            "A relative time offset only compares against a value that holds a timestamp.", path);
                    }
                    break;

                default:
                    // A literal needs no further checking: a null value is a legitimate comparison.
                    break;
            }
        }

        // Records hold their collections by reference, so two structurally identical calls are not
        // equal on their own. Grouping has to compare them by shape to know a selected call is one
        // of the grouping keys.
        private static bool SameCall(QueryFunctionCall left, QueryFunctionCall right)
        {
            if (!string.Equals(left.Function, right.Function, StringComparison.OrdinalIgnoreCase)) return false;
            if (left.Arguments.Count != right.Arguments.Count) return false;
            for (var i = 0; i < left.Arguments.Count; i++)
            {
                if (!SameOperand(left.Arguments[i], right.Arguments[i])) return false;
            }
            return true;
        }

        private static bool SameOperand(QueryOperand left, QueryOperand right)
        {
            if (left.Kind != right.Kind) return false;
            return left.Kind switch
            {
                QueryOperandKind.Literal => string.Equals(left.Value, right.Value, StringComparison.Ordinal),
                QueryOperandKind.Field => left.Field is not null && right.Field is not null
                    && string.Equals(left.Field.Alias, right.Field.Alias, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(left.Field.Field, right.Field.Field, StringComparison.OrdinalIgnoreCase),
                QueryOperandKind.Relative => Equals(left.Relative, right.Relative),
                QueryOperandKind.Function => left.Call is not null && right.Call is not null
                    && SameCall(left.Call, right.Call),
                QueryOperandKind.List => (left.Values ?? []).SequenceEqual(right.Values ?? []),
                _ => false,
            };
        }
    }
}
