using NovinDevHubStaffCore.Core.Activities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace NovinDevHubStaffCore.Core
{
    class XmlDataSource
    {

        readonly string applicationsFile = "Applications.xml";
        readonly string activitiesFile = "Activities.xml";
        readonly string browsedUrlsFile = "BrowsedUrls.xml";
        readonly string ScreenshotsFile = "Screenshots.xml";

        public List<ProccessActivity> ProccessUsages { get; private set; }
        public List<BrowserActivity> BrowsedUrls { get; private set; }
        public List<Models.Activity> MouseKeyboardActivies { get; private set; }
        public List<ScreenCapture> Screenshots { get; private set; }

        public XmlDataSource()
        {
            ProccessUsages = new List<ProccessActivity>();
            BrowsedUrls = new List<BrowserActivity>();
            MouseKeyboardActivies = new List<Models.Activity>();
            Screenshots = new List<ScreenCapture>();

            applicationsFile = Path.Combine(Environment.CurrentDirectory, "data", applicationsFile);
            activitiesFile = Path.Combine(Environment.CurrentDirectory, "data", activitiesFile);
            browsedUrlsFile = Path.Combine(Environment.CurrentDirectory, "data", browsedUrlsFile);
            ScreenshotsFile = Path.Combine(Environment.CurrentDirectory, "data", ScreenshotsFile);



            if (!File.Exists(applicationsFile))
                CreateEmptyFile(applicationsFile);


            if (!File.Exists(activitiesFile))
                CreateEmptyFile(activitiesFile);

            if (!File.Exists(browsedUrlsFile))
                CreateEmptyFile(browsedUrlsFile);

            if (!File.Exists(ScreenshotsFile))
                CreateEmptyFile(ScreenshotsFile);
        }


        public void SaveApplication()
        {
            XmlDocument doc = LoadVerifyXml(applicationsFile);
            if(doc==null)
            {
                MessageBox.Show("Applications file is modified and not valid");
                CreateEmptyFile(applicationsFile);
                doc = LoadVerifyXml(applicationsFile);
            }
            XmlNode root = doc.DocumentElement;


            foreach (var p in ProccessUsages)
            {
                //Create a new node.
                var app = CreateElement(doc, "Application", p);

                //Add the node to the document.
                root.AppendChild(app);
            }

            SaveSignedXmlFile(applicationsFile, doc);
            //Reset data
            ProccessUsages = new List<ProccessActivity>();
        }

        public void SaveActivity()
        {
            XmlDocument doc = LoadVerifyXml(activitiesFile);
            if (doc == null)
            {
                MessageBox.Show("Activities file is modified and not valid");
                CreateEmptyFile(activitiesFile);
                doc = LoadVerifyXml(activitiesFile);
            }
            XmlNode root = doc.DocumentElement;


            Debug.WriteLine("Display the modified XML...");

            foreach (var p in MouseKeyboardActivies)
            {
                //Create a new node.
                var app = CreateElement(doc, "Activity", p);

                //Add the node to the document.
                root.AppendChild(app);
            }

            SaveSignedXmlFile(activitiesFile, doc);
            //Reset data
            MouseKeyboardActivies = new List<Models.Activity>();
        }

        public void SaveBrowsedUrls()
        {
            XmlDocument doc = LoadVerifyXml(browsedUrlsFile);
            if (doc == null)
            {
                MessageBox.Show("Browsed Urls file is modified and not valid");
                CreateEmptyFile(browsedUrlsFile);
                doc = LoadVerifyXml(browsedUrlsFile);
            }
            XmlNode root = doc.DocumentElement;


            Debug.WriteLine("Display the modified XML...");

            foreach (var p in BrowsedUrls)
            {
                //Create a new node.
                var app = CreateElement(doc, "BrowsedUrl", p);

                //Add the node to the document.
                root.AppendChild(app);
            }

            SaveSignedXmlFile(browsedUrlsFile, doc);
            //Reset data
            BrowsedUrls = new List<BrowserActivity>();
        }

        public void SaveScreenshots()
        {
            XmlDocument doc = LoadVerifyXml(ScreenshotsFile);
            if (doc == null)
            {
                MessageBox.Show("Screenshots file is modified and not valid");
                CreateEmptyFile(ScreenshotsFile);
                doc = LoadVerifyXml(ScreenshotsFile);
            }
            XmlNode root = doc.DocumentElement;


            foreach (var p in Screenshots)
            {
                //Create a new node.
                var app = CreateElement(doc, "Screenshot", p);

                //Add the node to the document.
                root.AppendChild(app);
            }

            SaveSignedXmlFile(applicationsFile, doc);
            //Reset data
            Screenshots = new List<ScreenCapture>();
        }

        private XmlNode CreateElement(XmlDocument doc, string elementName, object obj)
        {
            //Create new node element by given name
            var node = doc.CreateElement(elementName);

            var type = obj.GetType();

            //Get all the properties of the object
            foreach (var p in type.GetProperties())
            {
                //Create sub element for each property of the object
                var el = doc.CreateElement(p.Name);
                //set value of the property to sub element
                var objValue = p.GetValue(obj);
                if (objValue != null)
                    el.InnerText = objValue.ToString();
                //add sub element to new node
                node.AppendChild(el);
            }

            return node;
        }

        private void CreateEmptyFile(string path)
        {
            // Create a new XML document.
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode rootNode = xmlDoc.CreateElement("Root"); // This overload assumes the document already knows about the rdf schema as it is in the Schemas set
            xmlDoc.AppendChild(rootNode);
            SaveSignedXmlFile(path, xmlDoc);
        }
        void SaveSignedXmlFile(string path, XmlDocument xmlDoc)
        {
            CspParameters cspParams = new CspParameters();
            cspParams.KeyContainerName = "XML_DSIG_RSA_KEY";

            // Create a new RSA signing key and save it in the container.
            RSACryptoServiceProvider rsaKey = new RSACryptoServiceProvider(cspParams);

            xmlDoc.PreserveWhitespace = true;
            //xmlDoc.Load(path);

            // Sign the XML document.
            SignXml(xmlDoc, rsaKey);

            Console.WriteLine("XML file signed.");

            // Save the document.
            xmlDoc.Save(path);
        }

        // Verify the signature of an XML file against an asymmetric
        // algorithm and return the result.
        private XmlDocument LoadVerifyXml(string path)
        {
            CspParameters cspParams = new CspParameters();
            cspParams.KeyContainerName = "XML_DSIG_RSA_KEY";

            // Create a new RSA signing key and save it in the container.
            RSACryptoServiceProvider rsaKey = new RSACryptoServiceProvider(cspParams);

            // Create a new XML document.
            XmlDocument xmlDoc = new XmlDocument();

            // Load an XML file into the XmlDocument object.
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.Load(path);

            // Verify the signature of the signed XML.

            // Check arguments.
            if (xmlDoc == null)
                throw new ArgumentException("xmlDoc");
            if (rsaKey == null)
                throw new ArgumentException("key");

            // Create a new SignedXml object and pass it
            // the XML document class.
            SignedXml signedXml = new SignedXml(xmlDoc);

            // Find the "Signature" node and create a new
            // XmlNodeList object.
            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("Signature");

            // Throw an exception if no signature was found.
            if (nodeList.Count <= 0)
            {
                throw new CryptographicException("Verification failed: No Signature was found in the document.");
            }

            // This example only supports one signature for
            // the entire XML document.  Throw an exception
            // if more than one signature was found.
            if (nodeList.Count >= 2)
            {
                throw new CryptographicException("Verification failed: More that one signature was found for the document.");
            }

            // Load the first <signature> node.
            signedXml.LoadXml((XmlElement)nodeList[0]);

            // Check the signature and return the result.
            if (signedXml.CheckSignature(rsaKey))
                return xmlDoc;
            else
                return null;
        }

        // Sign an XML file.
        // This document cannot be verified unless the verifying
        // code has the key with which it was signed.
        private void SignXml(XmlDocument xmlDoc, RSA rsaKey)
        {
            // Check arguments.
            if (xmlDoc == null)
                throw new ArgumentException(nameof(xmlDoc));
            if (rsaKey == null)
                throw new ArgumentException(nameof(rsaKey));

            XmlNodeList signNode = xmlDoc.GetElementsByTagName("Signature");
            if(signNode.Count>0)
                signNode.Item(0).ParentNode.RemoveChild(signNode.Item(0));

            // Create a SignedXml object.
            SignedXml signedXml = new SignedXml(xmlDoc);
          
            // Add the key to the SignedXml document.
            signedXml.SigningKey = rsaKey;

            // Create a reference to be signed.
            Reference reference = new Reference();
            reference.Uri = "";

            // Add an enveloped transformation to the reference.
            XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
            reference.AddTransform(env);

            // Add the reference to the SignedXml object.
            signedXml.AddReference(reference);

            // Compute the signature.
            signedXml.ComputeSignature();

            // Get the XML representation of the signature and save
            // it to an XmlElement object.
            XmlElement xmlDigitalSignature = signedXml.GetXml();

            // Append the element to the XML document.
            xmlDoc.DocumentElement.AppendChild(xmlDoc.ImportNode(xmlDigitalSignature, true));
        }
    }
}
