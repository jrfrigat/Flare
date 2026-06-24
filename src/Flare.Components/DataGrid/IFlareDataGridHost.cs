namespace Flare.Components;

/// <summary>
/// Interface for DataGrid host that allows child components (FlareColumn, FlareColumnBand) to register.
/// </summary>
internal interface IFlareDataGridHost
{
    void AddBand(FlareColumnBand band);
    void RemoveBand(FlareColumnBand band);
    void NotifyStructureChanged();
}
