using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Ionic.Zlib;
using KeePass.DataExchange;
using KeePass.Resources;
using KeePassLib;
using KeePassLib.Interfaces;
using KeePassLib.Security;

namespace SafeInCloudImp
{
    class SafeInCloud : FileFormatProvider
    {
        private const string ElemDatabase = "database";
        private const string ElemCard = "card";
        private const string ElemLabel = "label";
        private const string ElemTemplate = "template";
        private const string ElemId = "id";
        private const string ElemTitle = "title";
        private const string ElemTimeStamp = "time_stamp";
        private const string ElemFieldLabelId = "label_id";
        private const string ElemFieldNotes = "notes";
        private const string ElemFieldSimple = "field";
        private const string ElemFieldName = "name";
        private const string ElemFieldType = "type";

        private const string ElemFieldTypeUrl = "website";
        private const string ElemFieldTypeLogin = "login";
        private const string ElemFieldTypePassword = "password";
        private const string ElemFieldTypePin = "pin";
        private const string ElemFieldTypeSecret = "secret";
        private const string ElemFieldTypeFile = "file";
        private const string ElemFieldTypeImage = "image";
        private const string ElemFieldTypeOtp = "one_time_password";
        private const string ElemFieldTypeCustomIcon = "custom_icon";

        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1);

        public override bool SupportsImport { get { return true; } }
        public override bool SupportsExport { get { return false; } }
        public override string FormatName { get { return "SafeInCloud"; } }
        public override string DefaultExtension { get { return "xml|db"; } }
        public override string ApplicationGroup { get { return KPRes.PasswordManagers; } }
        public override bool ImportAppendsToRootGroupOnly { get { return false; } }

        public override Image SmallIcon
        {
            get { return Properties.Resources.B16x16_Imp_SafeInCloud; }
        }

        /// <summary>
        /// Labels, imported from SIC
        /// </summary>
        private readonly Dictionary<string, string> _labels = new Dictionary<string, string>();
        /// <summary>
        /// Local KeePass groups
        /// </summary>
        private readonly Dictionary<string, PwGroup> _groups = new Dictionary<string, PwGroup>();

        /// <summary>
        /// In SafeInCloud fields can be named equally, so trying make unique names
        /// </summary>
        class NameUniquizer
        {
            private readonly Dictionary<string, bool> _names = new Dictionary<string, bool>();

            public string MakeUnique(string name)
            {
                if (!_names.ContainsKey(name))
                {
                    _names[name] = true;
                    return name;
                }
                Int32 i = 2;
                string name2;
                while (_names.ContainsKey(name2 = name + i++))
                {
                }
                _names[name2] = true;
                return name2;
            }
        }

        public override void Import(PwDatabase pwStorage, Stream sInput, IStatusLogger slLogger)
        {
            try
            {
                _labels.Clear();
                _groups.Clear();

                var decodedStream = Decryptor.DecodeStream(sInput);
                if (decodedStream == null)
                    return;

                XmlDocument xd = new XmlDocument();
                xd.Load(decodedStream);
                decodedStream.Close();

                XmlNode xnRoot = xd.DocumentElement;
                if (xnRoot == null)
                    return;
                Debug.Assert(xnRoot.Name == ElemDatabase);
                foreach (XmlNode xn in xnRoot.ChildNodes)
                {
                    if (xn.NodeType == XmlNodeType.Element)
                    {
                        switch (xn.Name)
                        {
                            case ElemLabel:
                                ImportLabel(xn, pwStorage.RootGroup, pwStorage);
                                break;
                        }
                    }
                }
                foreach (XmlNode xn in xnRoot.ChildNodes)
                {
                    if (xn.NodeType == XmlNodeType.Element)
                    {
                        switch (xn.Name)
                        {
                            case ElemCard:
                                ImportCard(xn, pwStorage.RootGroup, pwStorage);
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Problem while importing XML file. " +
                    "Copy content of this dialog box by pressing Ctrl+C and send error report to author\n" +
                    "https://github.com/Alezy80/SIC2KeePass/issues\n\n{0}\n{1}",
                    ex.Message, ex.StackTrace.ToString()),
                    "Import error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Get attribute from XML, if attribute doesn't exist, returns defaultValue
        /// </summary>
        /// <param name="node">XML node</param>
        /// <param name="name">Name of attribute</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Attribute value or defaultValue, if attribute doesn't exist</returns>
        private static string GetAttribute(XmlNode node, string name, string defaultValue = null)
        {
            if (node == null || node.Attributes == null)
                return defaultValue;
            XmlAttribute attr = node.Attributes[name];
            return attr == null ? defaultValue : attr.InnerText;
        }

        private void ImportCard(XmlNode xnCard, PwGroup pgParent, PwDatabase pd)
        {
            if (xnCard.Attributes == null || Boolean.Parse(GetAttribute(xnCard, ElemTemplate, "false")))
                return;

            String id = GetAttribute(xnCard, ElemId, "0");
            String title = GetAttribute(xnCard, ElemTitle, "");
            DateTime time = UnixEpoch.AddSeconds(Int64.Parse(GetAttribute(xnCard, ElemTimeStamp, "0")) / 1000.0);
            Int32 iid = int.Parse(id);

            PwEntry pwEntry = new PwEntry(true, true);
            ImportUtil.AppendToField(pwEntry, PwDefs.TitleField, title, pd);
            pwEntry.LastModificationTime = time;
            pwEntry.CreationTime = time;
            ImportFields(xnCard, pwEntry, pd);
            if (pwEntry.Tags.Count > 0)
            {
                PwGroup group;
                if (_groups.TryGetValue(pwEntry.Tags[0], out group))
                    pgParent = group;
            }
            pgParent.AddEntry(pwEntry, true);
        }

        private static bool IsSecretField(string fieldType)
        {
            return
                fieldType == ElemFieldTypePassword ||
                fieldType == ElemFieldTypePin ||
                fieldType == ElemFieldTypeSecret ||
                fieldType == ElemFieldTypeOtp;
        }

        private string GetLabel(string id)
        {
            string label;
            if (_labels.TryGetValue(id, out label))
                return label;
            return id;
        }

        public static String DecodeQueryParameters(Uri uri, String key)
        {
            foreach (string item in uri.Query.Split('&'))
            {
                String[] parts = item.Replace("?", "").Split('=');
                if (parts.Length == 2 && parts[0].Equals(key, StringComparison.OrdinalIgnoreCase))
                    return parts[1];
            }
            return null;
        }

        private static String GetSecretKey(String text)
        {
            try
            {
                Uri uri = new Uri(text);
                if ((uri.Scheme == "otpauth") && (uri.Authority == "totp"))
                {
                    String str = DecodeQueryParameters(uri, "secret");
                    if (!String.IsNullOrEmpty(str))
                    {
                        String result = "key=" + str;
                        str = DecodeQueryParameters(uri, "period");
                        if (!String.IsNullOrEmpty(str))
                            result = result + "&step=" + str;
                        str = DecodeQueryParameters(uri, "digits");
                        if (!String.IsNullOrEmpty(str))
                            result = result + "&size=" + str;
                        return result;
                    }
                }
            }
            catch
            {
            }
            return "key=" + text;
        }

        private void ImportFields(XmlNode xnCard, PwEntry pwEntry, PwDatabase pd)
        {
            Boolean hasUser = false;
            Boolean hasUrl = false;
            Boolean hasPassword = false;
            Int32 imageNo = 1;
            List<string> labels = new List<string>();
            NameUniquizer nu = new NameUniquizer();

            foreach (XmlNode field in xnCard.ChildNodes)
            {
                switch (field.Name)
                {
                    case ElemFieldLabelId:
                        labels.Add(GetLabel(field.InnerText));
                        break;
                    case ElemFieldNotes:
                        ImportUtil.AppendToField(pwEntry, PwDefs.NotesField, field.InnerText, pd);
                        break;
                    case ElemFieldSimple:
                        String fieldName = GetAttribute(field, ElemFieldName, "");
                        String type = GetAttribute(field, ElemFieldType, "");
                        String value = field.InnerText;
                        if (!hasUser && type == ElemFieldTypeLogin)
                        {
                            fieldName = PwDefs.UserNameField;
                            hasUser = true;
                        }
                        else if (!hasUrl && type == ElemFieldTypeUrl)
                        {
                            fieldName = PwDefs.UrlField;
                            hasUrl = true;
                        }
                        else if (!hasPassword && type == ElemFieldTypePassword)
                        {
                            fieldName = PwDefs.PasswordField;
                            hasPassword = true;
                        }
                        else if (type == ElemFieldTypeOtp)
                        {
                            fieldName = "otp";
                            value = GetSecretKey(value);
                        }

                        fieldName = nu.MakeUnique(fieldName);
                        ImportUtil.AppendToField(pwEntry, fieldName, value, pd);
                        if (IsSecretField(type))
                            pwEntry.Strings.EnableProtection(fieldName, true);
                        break;
                    case ElemFieldTypeFile:
                        String fileName = GetAttribute(field, ElemFieldName, "");
                        Byte[] content = ZlibStream.UncompressBuffer(Convert.FromBase64String(field.InnerText));
                        pwEntry.Binaries.Set(fileName, new ProtectedBinary(false, content));
                        break;
                    case ElemFieldTypeImage:
                        String imageName = String.Format("image{0}.jpg", imageNo++);
                        Byte[] imageContent = Convert.FromBase64String(field.InnerText);
                        pwEntry.Binaries.Set(imageName, new ProtectedBinary(false, imageContent));
                        break;
                    case ElemFieldTypeCustomIcon:
                        break; //cann't  handle
                    default:
                        Debug.Assert(false, "Unknown field type " + field.Name);
                        break;
                }
            }
            pwEntry.Tags.AddRange(labels);
        }

        private void ImportLabel(XmlNode xnLabel, PwGroup pgParent, PwDatabase pd)
        {
            if (xnLabel.Attributes == null)
                return;

            XmlAttribute idAttr = xnLabel.Attributes["id"];
            XmlAttribute nameAttr = xnLabel.Attributes["name"];
            if (idAttr == null || nameAttr == null)
                return;

            String id = idAttr.InnerText;
            String name = nameAttr.InnerText;
            _labels[id] = name;

            if (_groups.ContainsKey(name))
                return;
            foreach (PwGroup pwGroup in pgParent.Groups)
            {
                if (pwGroup.Name == name) //do not add existing groups
                {
                    _groups[name] = pwGroup;
                    return;
                }
            }
            PwGroup pg = new PwGroup(true, true);
            pg.Name = name;
            pgParent.AddGroup(pg, true);
            _groups[name] = pg;
        }
    }
}
