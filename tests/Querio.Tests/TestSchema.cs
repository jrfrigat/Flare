namespace Querio.Tests;

/// <summary>
/// A schema shaped like a real one rather than a toy: a request log to aggregate over, plus the
/// relational cases that break naive query models - a self-relation (a user's manager), two separate
/// paths to the same entity (a transfer's sender and recipient) and a composite key.
/// </summary>
internal static class TestSchema
{
    internal static QuerySchema Build() => new(
        [
            new QueryEntity("requests", "Requests",
            [
                new QueryField("id", "Id", QueryFieldType.Guid),
                new QueryField("route", "Route", QueryFieldType.Text),
                new QueryField("timestamp", "Timestamp", QueryFieldType.DateTime),
                new QueryField("durationMs", "Duration, ms", QueryFieldType.Number),
                new QueryField("cacheHit", "Cache hit", QueryFieldType.Boolean),
                new QueryField("error", "Error", QueryFieldType.Boolean),
                new QueryField("status", "Status", QueryFieldType.Number),
                new QueryField("apiKeyId", "API key", QueryFieldType.Guid),
            ])
            { Source = "dbo.RequestLog", PrimaryKey = ["id"] },

            new QueryEntity("apiKeys", "API keys",
            [
                new QueryField("id", "Id", QueryFieldType.Guid),
                new QueryField("name", "Name", QueryFieldType.Text),
                new QueryField("ownerId", "Owner", QueryFieldType.Guid),
            ])
            { PrimaryKey = ["id"] },

            new QueryEntity("users", "Users",
            [
                new QueryField("id", "Id", QueryFieldType.Guid),
                new QueryField("name", "Name", QueryFieldType.Text),
                new QueryField("managerId", "Manager", QueryFieldType.Guid) { Nullable = true },
                // Deliberately restricted, so the validator has something to reject.
                new QueryField("secret", "Secret", QueryFieldType.Text) { Filterable = false, Groupable = false },
            ])
            { PrimaryKey = ["id"] },

            new QueryEntity("transfers", "Transfers",
            [
                new QueryField("id", "Id", QueryFieldType.Guid),
                new QueryField("fromUserId", "From", QueryFieldType.Guid),
                new QueryField("toUserId", "To", QueryFieldType.Guid),
                new QueryField("amount", "Amount", QueryFieldType.Number),
                new QueryField("createdAt", "Created", QueryFieldType.DateTime),
            ])
            { PrimaryKey = ["id"] },

            new QueryEntity("orders", "Orders",
            [
                new QueryField("tenantId", "Tenant", QueryFieldType.Guid),
                new QueryField("number", "Number", QueryFieldType.Number),
                new QueryField("placedAt", "Placed", QueryFieldType.DateTime),
            ])
            { PrimaryKey = ["tenantId", "number"] },

            new QueryEntity("orderLines", "Order lines",
            [
                new QueryField("tenantId", "Tenant", QueryFieldType.Guid),
                new QueryField("orderNumber", "Order", QueryFieldType.Number),
                new QueryField("sku", "SKU", QueryFieldType.Text),
                new QueryField("quantity", "Quantity", QueryFieldType.Number),
            ]),
        ],
        [
            QueryRelation.Simple("request_apiKey", "requests", "apiKeyId", "apiKeys", "id"),
            QueryRelation.Simple("apiKey_owner", "apiKeys", "ownerId", "users", "id"),

            // Self-relation: a user's manager is another user.
            QueryRelation.Simple("user_manager", "users", "managerId", "users", "id"),

            // Two distinct paths from the same entity to the same target.
            QueryRelation.Simple("transfer_sender", "transfers", "fromUserId", "users", "id"),
            QueryRelation.Simple("transfer_recipient", "transfers", "toUserId", "users", "id"),

            // Composite key: both columns have to match.
            new QueryRelation("order_lines", "orders", "orderLines",
            [
                new QueryFieldPair("tenantId", "tenantId"),
                new QueryFieldPair("number", "orderNumber"),
            ])
            { Cardinality = QueryCardinality.OneToMany },
        ],
        [
            // A value function with an optional tail, so arity has a range rather than a single number.
            new QueryFunction("calcTax", "Calculate tax", QueryFunctionKind.Value)
            {
                Parameters =
                [
                    new QueryFunctionParameter("amount", "Amount", QueryFieldType.Number),
                    new QueryFunctionParameter("rate", "Rate", QueryFieldType.Number) { Optional = true },
                ],
                ReturnType = QueryFieldType.Number,
                Source = "dbo.CalcTax",
            },

            // Maps onto a built-in rather than a user routine, which is what the physical name is for.
            new QueryFunction("upper", "Upper case", QueryFunctionKind.Value)
            {
                Parameters = [new QueryFunctionParameter("text", "Text", QueryFieldType.Text)],
                ReturnType = QueryFieldType.Text,
                Source = "UPPER",
            },

            // A table function contributes columns, so it participates exactly like an entity.
            new QueryFunction("activeUsers", "Active users", QueryFunctionKind.Table)
            {
                Parameters = [new QueryFunctionParameter("since", "Since", QueryFieldType.DateTime)],
                Columns =
                [
                    new QueryField("id", "Id", QueryFieldType.Guid),
                    new QueryField("name", "Name", QueryFieldType.Text),
                ],
                Source = "dbo.GetActiveUsers",
            },
        ]);
}
