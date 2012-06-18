using System;
using System.IO;
using System.Xml;
using Phantom.Core;

namespace PhantomContrib
{
    public class XmlUpdate
    {
        
        public void UpdateConfigFile(string masterFile, string updateFile, string destinationFolder)
        {
            if(string.IsNullOrEmpty(masterFile))
                throw new StringIsNullOrEmptyException("Master file needs to exist for successful transformation");

            if (!masterFile.EndsWith(".master.config", StringComparison.InvariantCultureIgnoreCase))
                throw new StringIsNullOrEmptyException("Master file needs to follow the naming convention ending with .master.config");

            if (string.IsNullOrEmpty(destinationFolder))
                throw new StringIsNullOrEmptyException("A destination folder is needed.");

            if (string.IsNullOrEmpty(updateFile))
            {
                UseMasterWithNoTranformation(masterFile, destinationFolder.Replace('\\', '/'));
                return;
            }


            var masterXmlDocument = LoadDocument(masterFile.Replace('\\', '/'));
            if(masterXmlDocument == null)
                throw new NullReferenceException("Problem reading master xml config file.");

            var updateXmlDocument = LoadDocument(updateFile.Replace('\\', '/'));
            if (updateXmlDocument == null)
                throw new NullReferenceException("Problem reading update xml config file.");

            namespaceManager = new XmlNamespaceManager(masterXmlDocument.NameTable);

            var updateRoot = "/";
            XmlNode updateRootNode = updateXmlDocument.SelectSingleNode(updateRoot, namespaceManager);

            var masterRoot = "/";
            XmlNode masterRootNode = masterXmlDocument.SelectSingleNode(masterRoot, namespaceManager);

            if (updateRootNode == null)
                throw new NullReferenceException(string.Format("Unable to locate '{0}' in {1}.", updateRoot, updateFile));

            if (masterRootNode == null)
                throw new NullReferenceException(string.Format("Unable to locate '{0}' in {1}.", masterRoot, masterFile));

            try
            {
                addAllChildNodes(masterXmlDocument, masterRootNode, updateRootNode);
            }
            catch (MultipleRootNodesException)
            {
                throw new NullReferenceException("Cannot create a new document root node because one already exists. Make sure to set the SubstitutionsRoot property.");
            }


            SaveDocumentToDestinationLocation(masterXmlDocument, masterFile, destinationFolder.Replace('\\', '/'));

        }

        private void SaveDocumentToDestinationLocation(XmlDocument masterXmlDocument, string masterFile, string destinationFolder)
        {
            var fileName = GetDesitinationFileNameFromMaster(masterFile);
            masterXmlDocument.Save(Path.Combine(destinationFolder, fileName));
        }

        private string GetDesitinationFileNameFromMaster(string masterFile)
        {
            if (!masterFile.EndsWith(".master.config", StringComparison.InvariantCultureIgnoreCase))
                throw new ApplicationException("Master file must end with .master.config");

            var onlyFileName = Path.GetFileName(masterFile);

            return onlyFileName.Substring(0, onlyFileName.Length - ".master.config".Length) + ".config";
        }

        private void UseMasterWithNoTranformation(string masterFile, string destinationFolder)
        {
            var xmlFile = LoadDocument(masterFile.Replace('\\', '/'));
            xmlFile.Save(Path.Combine(destinationFolder, GetDesitinationFileNameFromMaster(masterFile)));
        }

        private void addAllChildNodes(XmlDocument mergedDocument, XmlNode contentParentNode, XmlNode substitutionsParentNode)
        {
            XmlNode substitutionNode = substitutionsParentNode.FirstChild;
            while (substitutionNode != null)
            {
                switch (substitutionNode.NodeType)
                {
                    case XmlNodeType.Element:
                        if (shouldDeleteElement(substitutionNode))
                        {
                            removeChildNode(mergedDocument, contentParentNode, substitutionNode);
                        }
                        else
                        {
                            XmlNode mergedNode = addChildNode(mergedDocument, contentParentNode, substitutionNode);
                            addAllChildNodes(mergedDocument, mergedNode, substitutionNode);
                        }
                        break;
                    case XmlNodeType.Text:
                        contentParentNode.InnerText = substitutionNode.Value;
                        break;
                    case XmlNodeType.CDATA:
                        contentParentNode.RemoveAll();
                        contentParentNode.AppendChild(mergedDocument.CreateCDataSection(substitutionNode.Value));
                        break;
                    default:
                        break;
                }
                substitutionNode = substitutionNode.NextSibling;
            }
        }


        private void removeChildNode(XmlDocument mergedDocument, XmlNode contentParentNode, XmlNode substitutionNode)
        {
            modifyNode(mergedDocument, contentParentNode, substitutionNode, true);
        }

        private XmlNode addChildNode(XmlDocument mergedDocument, XmlNode destinationParentNode, XmlNode nodeToAdd)
        {
            XmlNode newNode = modifyNode(mergedDocument, destinationParentNode, nodeToAdd);

            foreach (XmlAttribute sourceAttribute in nodeToAdd.Attributes)
            {
                if (!isUpdateControlAttribute(sourceAttribute))
                {
                    setAttributeValue(mergedDocument, newNode, sourceAttribute.Name, sourceAttribute.Value);
                }
            }
            return newNode;
        }

        private void setAttributeValue(XmlDocument mergedDocument, XmlNode modifiedNode, string attributeName, string attributeValue)
        {
            XmlAttribute targetAttribute = modifiedNode.Attributes[attributeName];
            if (targetAttribute == null)
            {
                Console.WriteLine("Creating attribute '{0}' on '{1}'", attributeName, getFullPathOfNode(modifiedNode));
                targetAttribute = modifiedNode.Attributes.Append(mergedDocument.CreateAttribute(attributeName));
            }
            targetAttribute.Value = attributeValue;
            Console.WriteLine("Setting attribute '{0}' to '{1}' on '{2}'", targetAttribute.Name, targetAttribute.Value, getFullPathOfNode(modifiedNode));
        }

        private static bool isUpdateControlAttribute(XmlAttribute attribute)
        {
            // check for the control namespace declaration
            if ((attribute.Prefix == "xmlns") && (attribute.Value == updateControlNamespace)) return true;
            // check for attributes within the control namespace
            if (attribute.NamespaceURI == updateControlNamespace) return true;
            return false;
        }

        private bool shouldDeleteElement(XmlNode sourceNode)
        {
            string action = getActionAttribute(sourceNode);
            return action.Equals("remove", StringComparison.InvariantCultureIgnoreCase);
        }

        private XmlNode modifyNode(XmlDocument mergedDocument, XmlNode destinationParentNode, XmlNode nodeToModify)
        {
            return modifyNode(mergedDocument, destinationParentNode, nodeToModify, false);
        }

        private XmlNamespaceManager namespaceManager;
        private XmlNode locateTargetNode(XmlNode parentNode, XmlNode nodeToFind, XmlAttribute keyAttribute)
        {
            string xpath;
            if (keyAttribute == null)
            {
                xpath = nodeToFind.Name;
            }
            else
            {
                Console.WriteLine("Using keyed attribute '{0}={1}' to locate node '{2}'", keyAttribute.LocalName, keyAttribute.Value, getFullPathOfNode(parentNode) + "/" + nodeToFind.LocalName);
                xpath = String.Format("{0}[@{1}='{2}']", nodeToFind.LocalName, keyAttribute.LocalName, keyAttribute.Value);
            }
            XmlNode foundNode = parentNode.SelectSingleNode(xpath, namespaceManager);
            return foundNode;
        }


        private XmlNode modifyNode(XmlDocument mergedDocument, XmlNode destinationParentNode, XmlNode nodeToModify, bool remove)
        {
            XmlAttribute keyAttribute = getKeyAttribute(nodeToModify);
            XmlNode targetNode = locateTargetNode(destinationParentNode, nodeToModify, keyAttribute);
            if (targetNode == null)
            {
                if (remove) return null;
                if (destinationParentNode.NodeType == XmlNodeType.Document)
                {
                    throw new MultipleRootNodesException();
                }
                targetNode = destinationParentNode.AppendChild(mergedDocument.CreateNode(XmlNodeType.Element, nodeToModify.Name, String.Empty));
                Console.WriteLine("Created node '{0}'", getFullPathOfNode(targetNode));
                if (keyAttribute != null)
                {
                    XmlAttribute keyAttributeOnNewNode = targetNode.Attributes.Append(mergedDocument.CreateAttribute(keyAttribute.LocalName));
                    keyAttributeOnNewNode.Value = keyAttribute.Value;
                }
            }
            else
            {
                if (remove)
                {
                    Console.WriteLine("Removing node '{0}'", getFullPathOfNode(targetNode));
                    destinationParentNode.RemoveChild(targetNode);
                }
            }
            return targetNode;
        }

   

        private XmlAttribute getKeyAttribute(XmlNode sourceNode)
        {
            XmlAttribute keyAttribute = null;
            for (int i = 0; i < sourceNode.Attributes.Count; i++)
            {
                if ((sourceNode.Attributes[i].NamespaceURI == updateControlNamespace) &&
                    (sourceNode.Attributes[i].LocalName == "key"))
                {
                    string keyAttributeName = sourceNode.Attributes[i].Value;
                    keyAttribute = sourceNode.Attributes[keyAttributeName];
                    break;
                }
            }
            return keyAttribute;
        }

        private string getActionAttribute(XmlNode sourceNode)
        {
            XmlNamespaceManager specialNamespaces = new XmlNamespaceManager(sourceNode.OwnerDocument.NameTable);
            specialNamespaces.AddNamespace("xmu", updateControlNamespace);
            XmlNode actionNode = sourceNode.SelectSingleNode("@xmu:action", specialNamespaces);
            if (actionNode == null) return String.Empty;
            return actionNode.Value;
        }

        private class MultipleRootNodesException : Exception { }

        private const string updateControlNamespace = "urn:msbuildcommunitytasks-xmlmassupdate";

        private string getFullPathOfNode(XmlNode node)
        {
            string fullPath = String.Empty;
            XmlNode currentNode = node;
            while (currentNode != null && currentNode.NodeType != XmlNodeType.Document)
            {
                fullPath = "/" + currentNode.Name + fullPath;
                currentNode = currentNode.ParentNode;
            }
            return fullPath;
        }

        protected virtual XmlDocument LoadDocument(string pathToConfigFile)
        {
            if (!System.IO.File.Exists(pathToConfigFile))
            {
                Console.WriteLine("Unable to load content file {0}", pathToConfigFile);
                return null;
            }
            XmlDocument contentDocument = new XmlDocument();
            contentDocument.Load(pathToConfigFile);
            return contentDocument;
        }
    }
}
