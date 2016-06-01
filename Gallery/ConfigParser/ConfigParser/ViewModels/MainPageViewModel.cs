using System.Collections.ObjectModel;
using System.Linq;
using System.Xml;
using ConfigParser.Utils;

namespace ConfigParser.ViewModels
{
    public class MainPageViewModel
    {
        private XmlDocument _sourceDocument { get; set; }
        private string _sourceFilePath { get; set; }
        private XmlDocument _destinationDocument { get; set; }
        private string _destinationFilePath { get; set; }

        public ObservableCollection<XmlNode> DestinationSourceCollection { get; set; }
        public ObservableCollection<XmlNode> SourceCollection { get; set; }

        private readonly CompareUtility _compareUtility;

        public MainPageViewModel()
        {
            DestinationSourceCollection = new ObservableCollection<XmlNode>();
            SourceCollection = new ObservableCollection<XmlNode>();
            _compareUtility = new CompareUtility();
        }

        public void OpenDestinationFile()
        {
            _destinationDocument = _compareUtility.OpenFile(DestinationSourceCollection, _destinationFilePath);
        }


        public void OpenSourceFile()
        {
            _sourceDocument = _compareUtility.OpenFile(SourceCollection, _sourceFilePath);
        }

        public void Save()
        {
            _sourceDocument?.Save(_sourceFilePath);
            _destinationDocument?.Save(_destinationFilePath);
        }

        public void SourceSelectionChanged(XmlNode selectedNode)
        {
            if (_destinationDocument != null)
            {
                _compareUtility.DisplaySimilarNode(selectedNode, DestinationSourceCollection, _destinationDocument);
            }
        }


        public void Diff()
        {
            var node = SourceCollection.First();
            SourceCollection.Clear();
            DestinationSourceCollection.Clear();
            _compareUtility.Diff(node, _destinationDocument, SourceCollection, DestinationSourceCollection);
        }
    }
}
