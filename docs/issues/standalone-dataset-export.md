# Export: standalone dataset export, not tied to a grid

**Status: OPEN. Feature request from a real app (OrderingPlatform, 0.26.2).**

The export pipeline (`IDataGridExporter<TItem>`, `DataGridExportData`, the toolbar-button
flow) is grid-centric by design: the grid hands the exporter ITS columns and ITS rows
("already sorted/filtered by the grid"). That is the right shape for "export what I see",
but two equally common needs have no library answer:

1. **Export the full dataset behind a filtered view.** The app's task card shows orders
   for one selected product x DC pair, while the CSV button must export ALL orders of the
   task. The grid's row set is the wrong source, so the export is built from the data
   service, not the grid.
2. **Export a dataset that has no grid at all** (distribution graph rows, a computed
   summary, a report assembled from several endpoints).

The app ended up with its own ~50-line CsvExporter: headers + row projections ->
string (UTF-8 BOM, ";" separator, ru date/number formats - the local Excel opens it
without an import wizard), plus a JS download call. Meanwhile the library already HAS the
right building blocks (the exporters' escaping with a CSV-injection guard,
`IFlareDownload`) - they are just unreachable without a grid.

## Ask

A grid-independent export surface, e.g.:

```csharp
// one static builder, same escaping rules as CsvGridExporter
string csv = FlareCsv.Build(headers, rows, FlareCsvOptions.ExcelRu); // BOM, ';', injection guard
await download.DownloadCsvAsync("orders-19.csv", csv);
```

- Options for delimiter/BOM (Excel in ru locale needs ";" + BOM; RFC 4180 "," for
  programmatic use) - the standard exporter is comma-only today.
- Formatters per column (the grid path has `DataGridExportColumn.Text`; the standalone
  path needs the same hook).
