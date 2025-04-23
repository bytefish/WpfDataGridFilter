using System.Windows;

namespace WpfDataGridFilter.Infrastructure
{
    /// <summary>
    /// Represents a method that handles general events.
    /// </summary>
    /// <typeparam name="TSender">The type of the sender.</typeparam>
    /// <typeparam name="TArgs">The type of the event data.</typeparam>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">An object that contains the event data.</param>
    public delegate void TypedEventHandler<in TSender, in TArgs>(TSender sender, TArgs args)
        where TSender : DependencyObject
        where TArgs : RoutedEventArgs;
}
