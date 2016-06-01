using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.XPath;
using ConfigParser.Utils;
using ConfigParser.ViewModels;
using Microsoft.Win32;

namespace ConfigParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainPageViewModel _viewModel;
        public static List<XmlAttribute> DiffAttr { get; set; }
        public static List<XmlAttribute> ChangedAttr { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainPageViewModel();
            DataContext = _viewModel;

            DiffAttr = new List<XmlAttribute>();
            ChangedAttr = new List<XmlAttribute>();
        }

        private void Button_OpenDestinationConfig(object sender, RoutedEventArgs e)
        {
            _viewModel.OpenDestinationFile();
        }

        private void Button_OpenSourceConfig(object sender, RoutedEventArgs e)
        {
            _viewModel.OpenSourceFile();
        }

        private void Button_SaveSourceConfig(object sender, RoutedEventArgs e)
        {
            _viewModel.Save();
        }


        private void IerarchySource_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var selectedNode = SourceTree.SelectedItem as XmlNode;
            _viewModel.SourceSelectionChanged(selectedNode);
        }

   




        private XmlNamespaceManager GetNameSpaceManager(XmlDocument xDoc)
        {
            XmlNamespaceManager nsm = new XmlNamespaceManager(xDoc.NameTable);
            XPathNavigator RootNode = xDoc.CreateNavigator();
            RootNode.MoveToFollowing(XPathNodeType.Element);
            IDictionary<string, string> NameSpaces = RootNode.GetNamespacesInScope(XmlNamespaceScope.All);

            foreach (KeyValuePair<string, string> kvp in NameSpaces)
            {
                nsm.AddNamespace(kvp.Key, kvp.Value);
            }

            return nsm;
        }
        private void ExpandTreeNodes(XmlNode destNode)
        {
            List<XmlNode> nodeList = new List<XmlNode>();

            nodeList.Insert(0, destNode);
            while (destNode.ParentNode != null)
            {
                destNode = destNode.ParentNode;
                if(destNode.NodeType != XmlNodeType.Document)
                nodeList.Insert(0, destNode);
            }

            foreach (object item in this.DestinationTree.Items)
            {
                TreeViewItem treeItem = this.DestinationTree.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                if (treeItem != null)
                    ExpandAll(treeItem, nodeList);

                TreeViewItem item2 = treeItem as TreeViewItem;
                if (item2 != null)
                    item2.IsExpanded = true;
            }
        }
        

        private void ExpandAll(ItemsControl items, List<XmlNode> expandList)
        {
            //var search = expandList.First();
            //expandList.Remove(search);
            foreach (object obj in items.Items)
            {
                if (expandList.Contains(obj))
                {
                    ItemsControl childControl = items.ItemContainerGenerator.ContainerFromItem(obj) as ItemsControl;
                    if (childControl != null)
                    {
                        ExpandAll(childControl, expandList);
                    }
                    TreeViewItem item = childControl as TreeViewItem;
                    if (item != null)
                        item.IsExpanded = true;
                }
            }
        }


       

        private void Button_DiffDestinationConfig(object sender, RoutedEventArgs e)
        {
            _viewModel.Diff();
        }


        

        private void Button_UpdateDestConfig(object sender, RoutedEventArgs e)
        {
            List<XmlNode> aList = new List<XmlNode>();

            foreach (XmlNode xmlNode in SourceTree.Items)
            {
                foreach (XmlNode destNode in DestinationTree.Items)
                {
                    if (xmlNode.Name == destNode.Name)
                    {
                        foreach (XmlAttribute xmlAttribute in xmlNode.Attributes)
                        {
                            if (destNode.Attributes != null && destNode.Attributes[xmlAttribute.Name] != null && destNode.Attributes[xmlAttribute.Name].Value != xmlAttribute.Value)
                            {
                                destNode.Attributes[xmlAttribute.Name].Value = xmlAttribute.Value;

                                ChangedAttr.Add(destNode.Attributes[xmlAttribute.Name]);
                                DiffAttr.Remove(destNode.Attributes[xmlAttribute.Name]);
                            }
                        }
                        aList.Add(destNode);
                    }
                }
            }
            _viewModel.DestinationSourceCollection.Clear();
            foreach (XmlNode xmlNode in aList)
            {
                _viewModel.DestinationSourceCollection.Add(xmlNode);
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (object item in this.DestinationTree.Items)
            {
                TreeViewItem treeItem =
                    this.DestinationTree.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                if (treeItem != null)
                {
                    ExpandAll(treeItem, true);
                    treeItem.IsExpanded = true;
                }
            }

            foreach (object item in this.SourceTree.Items)
            {
                TreeViewItem treeItem = this.SourceTree.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                if (treeItem != null)
                {
                    ExpandAll(treeItem, true);
                    treeItem.IsExpanded = true;
                }
            }
        }

        private void ExpandAll(ItemsControl items, bool expand)
        {
            foreach (object obj in items.Items)
            {
                ItemsControl childControl = items.ItemContainerGenerator.ContainerFromItem(obj) as ItemsControl;
                if (childControl != null)
                {
                    ExpandAll(childControl, expand);
                }
                TreeViewItem item = childControl as TreeViewItem;
                if (item != null)
                    item.IsExpanded = true;
            }
        }
    }
}
