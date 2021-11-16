using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Microsoft.Expression.Controls
{
	internal static class Utilities
	{
		public static Panel GetItemsHost(this ItemsControl control)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			DependencyObject dependencyObject = control.ItemContainerGenerator.ContainerFromIndex(0);
			if (dependencyObject != null)
			{
				return VisualTreeHelper.GetParent(dependencyObject) as Panel;
			}
			FrameworkElement frameworkElement = control.GetVisualChildren().FirstOrDefault<DependencyObject>() as FrameworkElement;
			if (frameworkElement != null)
			{
				ItemsPresenter itemsPresenter = frameworkElement.GetLogicalDescendents().OfType<ItemsPresenter>().FirstOrDefault<ItemsPresenter>();
				if (itemsPresenter != null && VisualTreeHelper.GetChildrenCount(itemsPresenter) > 0)
				{
					return VisualTreeHelper.GetChild(itemsPresenter, 0) as Panel;
				}
			}
			return null;
		}

		internal static IEnumerable<T> TraverseBreadthFirst<T>(T initialNode, Func<T, IEnumerable<T>> getChildNodes, Func<T, bool> traversePredicate)
		{
			Queue<T> queue = new Queue<T>();
			queue.Enqueue(initialNode);
			while (queue.Count > 0)
			{
				T node = queue.Dequeue();
				if (traversePredicate(node))
				{
					yield return node;
					IEnumerable<T> childNodes = getChildNodes(node);
					foreach (T t in childNodes)
					{
						queue.Enqueue(t);
					}
				}
			}
			yield break;
		}

		public static IEnumerable<DependencyObject> GetVisualChildren(this DependencyObject element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return element.GetVisualChildrenAndSelfIterator().Skip(1);
		}

		private static IEnumerable<DependencyObject> GetVisualChildrenAndSelfIterator(this DependencyObject element)
		{
			yield return element;
			int count = VisualTreeHelper.GetChildrenCount(element);
			for (int i = 0; i < count; i++)
			{
				yield return VisualTreeHelper.GetChild(element, i);
			}
			yield break;
		}

		internal static IEnumerable<FrameworkElement> GetLogicalChildren(this FrameworkElement parent)
		{
			Popup popup = parent as Popup;
			if (popup != null)
			{
				FrameworkElement popupChild = popup.Child as FrameworkElement;
				if (popupChild != null)
				{
					yield return popupChild;
				}
			}
			ItemsControl itemsControl = parent as ItemsControl;
			if (itemsControl != null)
			{
				foreach (FrameworkElement logicalChild in (from index in Enumerable.Range(0, itemsControl.Items.Count)
														   select itemsControl.ItemContainerGenerator.ContainerFromIndex(index)).OfType<FrameworkElement>())
				{
					yield return logicalChild;
				}
			}
			string name = parent.Name;
			Queue<FrameworkElement> queue = new Queue<FrameworkElement>(parent.GetVisualChildren().OfType<FrameworkElement>());
			while (queue.Count > 0)
			{
				FrameworkElement element = queue.Dequeue();
				if (element.Parent == parent || element is UserControl)
				{
					yield return element;
				}
				else
				{
					foreach (FrameworkElement frameworkElement in element.GetVisualChildren().OfType<FrameworkElement>())
					{
						queue.Enqueue(frameworkElement);
					}
				}
			}
			yield break;
		}

		internal static IEnumerable<FrameworkElement> GetLogicalDescendents(this FrameworkElement parent)
		{
			return Utilities.TraverseBreadthFirst<FrameworkElement>(parent, (FrameworkElement node) => node.GetLogicalChildren(), (FrameworkElement node) => true);
		}
	}
}
