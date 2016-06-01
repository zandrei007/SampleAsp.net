using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Win32;

namespace ConfigParser.Utils
{
    public class CompareUtility
    {
        public object CompareNodes(XmlDocument sourceDocument, XmlDocument destinationDocument)
        {
            
            //foreach (XmlNode sourceNode in sourceDocument)
            //{
            //    sourceNode.
            //}
            return null;
        }


        private string FindXPath(XmlNode node)
        {
            StringBuilder builder = new StringBuilder();
            while (node != null)
            {
                switch (node.NodeType)
                {
                    case XmlNodeType.Attribute:
                        builder.Insert(0, "/@" + node.Name);
                        node = ((XmlAttribute)node).OwnerElement;
                        break;
                    case XmlNodeType.Element:
                        int index = FindElementIndex((XmlElement)node);
                        builder.Insert(0, "/" + node.Name + "[" + index + "]");
                        node = node.ParentNode;
                        break;
                    case XmlNodeType.Document:
                        return builder.ToString();
                        //default:
                        //    throw new ArgumentException("Only elements and attributes are supported");
                }
            }
            throw new ArgumentException("Node was not in a document");
        }

        private XmlNode GetNodeByXpath(XmlDocument destinationDocument, string xPath)
        {
            var paths = xPath.Split('/');

            var node = destinationDocument.SelectSingleNode("/" + paths[1]);

            for (int i = 2; i < paths.Length; i++)
            {
                var names = paths[i].Split('[');
                var n2 = names[1].Split(']');

                var name = names.First();
                int index = 0;
                int.TryParse(n2.First(), out index);
                var list = GetXmlList(node, name);
                index--;

                if (list.Count == 0)
                {
                    return null;
                }

                node = list[index];
            }

            return node;
        }

        private List<XmlNode> GetXmlList(XmlNode node, string name)
        {
            var list = new List<XmlNode>();
            foreach (XmlNode n in node.ChildNodes)
            {
                if (n.Name == name)
                {
                    list.Add(n);
                }
            }
            return list;
        }



        static int FindElementIndex(XmlElement element)
        {
            XmlNode parentNode = element.ParentNode;
            if (parentNode is XmlDocument)
            {
                return 1;
            }
            XmlElement parent = (XmlElement)parentNode;
            int index = 1;
            foreach (XmlNode candidate in parent.ChildNodes)
            {
                if (candidate is XmlElement && candidate.Name == element.Name)
                {
                    if (candidate == element)
                    {
                        return index;
                    }
                    index++;
                }
            }
            throw new ArgumentException("Couldn't find element within parent");
        }



        public XmlDocument OpenFile(ObservableCollection<XmlNode> collection, string savePath)
        {
            collection.Clear();
            var openFileDialog = new OpenFileDialog();
            XmlDocument document = null;
            if (openFileDialog.ShowDialog() == true)
            {
                document = new XmlDocument();
                try
                {
                    savePath = openFileDialog.FileName;
                    document.Load(savePath);
                    XmlNodeList xnList = document.SelectNodes("/configuration");
                    foreach (XmlNode node in xnList)
                    {
                        collection.Add(node);
                    }
                }
                catch (Exception)
                {
                    // Todo: handle exception
                }
            }
            return document;
        }

        public void DisplaySimilarNode(XmlNode selectedNode, ObservableCollection<XmlNode> destinationSource, XmlDocument document)
        {
            //XmlNamespaceManager ns = GetNameSpaceManager(_sourceDocument);
            var xPath = FindXPath(selectedNode);
            //var destNode = _destinationDocument.SelectSingleNode(xPath, ns);
            var destNode = GetNodeByXpath(document, xPath);
            destinationSource.Clear();
            destinationSource.Add(destNode);
            if (destNode != null)
            {
                // todo: implement Expand
                //ExpandTreeNodes(destNode);
            }
        }




        //private void DeepSearch(XmlNode node)
        //{
        //    var xPath = FindXPath(node);
        //    var destNode = GetNodeByXpath(_destinationDocument, xPath);
        //    if(destNode != null)
        //        DeepCompare(node, destNode);
        //}

        private void DeepCompare(XmlNode source, XmlNode destination, ObservableCollection<XmlNode> sourceCollection, ObservableCollection<XmlNode> destinationCollection, XmlDocument document)
        {
            if (source.Name == destination.Name)
            {
                if (source.Attributes.Count > 0)
                {
                    foreach (XmlAttribute attribute in source.Attributes)
                    {
                        bool contains = false;
                        foreach (
                            XmlAttribute a in
                                from XmlAttribute a in destination.Attributes where a.Name == attribute.Name select a)
                        {
                            contains = true;
                        }

                        if (contains && destination.Attributes != null &&
                            destination.Attributes[attribute.Name].Value != attribute.Value)
                        {
                            if (!sourceCollection.Contains(source))
                            {
                                sourceCollection.Add(source);
                                destinationCollection.Add(destination);
                            }

                            MainWindow.DiffAttr.Add(destination.Attributes[attribute.Name]);
                        }
                    }
                }

                if (source.HasChildNodes)
                {
                    foreach (XmlNode childNode in source.ChildNodes)
                    {
                        if (childNode.NodeType != XmlNodeType.Comment)
                        {
                            Diff(childNode, document, sourceCollection, destinationCollection);
                        }
                    }
                }

            }
        }

        public void Diff(XmlNode node, XmlDocument destinationDocument, ObservableCollection<XmlNode> sourceCollection, ObservableCollection<XmlNode> destinationSourceCollection)
        {
            var xPath = FindXPath(node);
            var destNode = GetNodeByXpath(destinationDocument, xPath);
            if (destNode != null)
                DeepCompare(node, destNode, sourceCollection, destinationSourceCollection, destinationDocument);
        }
    }
}
