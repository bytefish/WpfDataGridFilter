# WpfDataGridFilter #

"I just want to filter some data in a DataGrid, why is all this so complicated?"... said everyone using a WPF DataGrid.

This library simplifies 

## What's included


## Using it:

You just need to add a `FilterableDataGridColumnHeader` as the `DataGridColumn.Header` for the Properties we want to Filter:

```xml

```

We can then subscribe to changes on a `FilterState` like this:

```csharp
```

And here is how to use the `FilterState` in a ViewModel:

```csharp
```
