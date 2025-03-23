# WpfDataGridFilter #

"I just want to filter some data in a `DataGrid`, why is all this so complicated?"... said everyone using a WPF `DataGrid`.

We just need to add a `FilterableDataGridColumnHeader` as the `DataGridColumn.Header` for the Properties we want to Filter:

```xml
<DataGrid ItemsSource="{Binding ViewModel.People}" AutoGenerateColumns="False">
    <DataGrid.Columns>
        <DataGridTextColumn Binding="{Binding PersonID}">
            <DataGridTextColumn.Header>
                <wpfdatagridfilter:FilterableDataGridColumnHeader FilterState="{Binding FilterState}" HeaderText="PersonID" PropertyName="PersonID" Height="30" FilterType="IntNumericFilter"></wpfdatagridfilter:FilterableDataGridColumnHeader>
            </DataGridTextColumn.Header>
        </DataGridTextColumn>
        <DataGridTextColumn Binding="{Binding FullName}">
            <DataGridTextColumn.Header>
                <wpfdatagridfilter:FilterableDataGridColumnHeader FilterState="{Binding FilterState}" HeaderText="FullName" PropertyName="FullName" Height="30" FilterType="StringFilter"></wpfdatagridfilter:FilterableDataGridColumnHeader>
            </DataGridTextColumn.Header>
        </DataGridTextColumn>
    </DataGrid.Columns>
</DataGrid>
```

This library adds 

![alt text](https://github.com/bytefish/WpfDataGridFilter/blob/main/doc/filter-datagridcolumnheader.jpg?raw=true)