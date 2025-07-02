using iManage.WorkSite.Web.ServicesProxy.IWOVService;
using Imanage.Documents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static Imanage.DocumentProfileItems;

namespace Imanage
{
    public class ImanageHelpers
    { 
        
        public static string[] GetDocNumberVersion(string objectId)
        {
            // example id:
            // !nrtdms:0:!session:gsoimandev1:!database:GSO:!document:18990301,1:
            var r = new Regex(@"(\d*),(\d*)", RegexOptions.IgnoreCase);
            var match = r.Match(objectId);

            if (match.Groups.Count == 3)
                return new string[] { match.Groups[1].Value, match.Groups[2].Value };

            return null;
        }

        public static void ThrowImanageError(IImanageObjectId[] imanageInputObjectIds, ImanageError[] imanageErrors)
        {
            if (imanageErrors == null || imanageErrors.Length == 0)
                return;

            var errorMessages = new List<string>();
            var documentObjectIds = new List<string>();

            if (imanageInputObjectIds != null)
                foreach (var imanageObjectId in imanageInputObjectIds)
                    documentObjectIds.Add(imanageObjectId.GetObjectId());
            
            foreach (var imanageError in imanageErrors)
            {
                if (imanageError == null) continue;

                var errorMessage = imanageError.Message;
                var profileErrors = new List<string>();

                if (imanageError.ImanageProfileErrors != null)
                    foreach (var profileError in imanageError.ImanageProfileErrors)
                        if (profileError != null)
                            profileErrors.Add(profileError.AttributeId.ToString() + ":" + profileError.Message);
                if (!string.IsNullOrEmpty(errorMessage))
                    errorMessages.Add("\'" + errorMessage + "\' > {" + string.Join(",", profileErrors) + "}");
            }
            if (errorMessages.Count() > 0)
                throw new Exception("Interwoven Error[objectId=" + string.Join(",", documentObjectIds) + "]: " + string.Join(" | ", errorMessages));
        }

        public static void ThrowImanageError(IImanageObjectId imanageInputObjectId, ImanageError[] imanageErrors)
        {
            ThrowImanageError(new IImanageObjectId[] { imanageInputObjectId }, imanageErrors);
        }

        public static ImanageDocumentNrl CreateNrlLink(ImanageDocumentObjectId objectId, string fileName)
        {
            if (objectId == null) return null;

            var nrlLinkFilename = "blank";
            var nrlLink = new ImanageDocumentNrl();
            nrlLink.Data = Encoding.UTF8.GetBytes(
                    objectId.Session + Environment.NewLine +
                    objectId.GetObjectId() + Environment.NewLine +
                    "[Version]" + Environment.NewLine +
                    "Latest=Y"
                    );

            if (!string.IsNullOrEmpty(fileName))
            {
                var pattern = "[" + Regex.Escape(new string(Path.GetInvalidFileNameChars())) + "]";
                var matches = Regex.Matches(fileName, pattern);

                foreach (Match match in matches)
                    fileName = fileName.Replace(match.Value, "");

                nrlLinkFilename = fileName.Trim();
            }

            nrlLink.FileName = nrlLinkFilename + ImanageDocument.EXTENSION_NRL;

            return nrlLink;
        }

        private static imSecurityType GetImSecurityType(ImanageSecurityObject.SecurityType securityType)
        {
            imSecurityType security = imSecurityType.imPublic;

            switch (securityType)
            {
                case ImanageSecurityObject.SecurityType.PRIVATE:
                    security = imSecurityType.imPrivate;
                    break;
                case ImanageSecurityObject.SecurityType.VIEW:
                    security = imSecurityType.imView;
                    break;
                default:
                    break;
            }

            return security;
        }

        public static ProfileAttributeId[] BuildProfileAttributeIds(string[] outputProfileIds)
        {
            if (outputProfileIds == null || outputProfileIds.Length == 0) return null;

            var profileAttributeIds = new List<ProfileAttributeId>();
            foreach (var outputProfileId in outputProfileIds)
            {
                ProfileAttributeId profileAttributeId;
                if (Enum.TryParse(outputProfileId, true, out profileAttributeId))
                    profileAttributeIds.Add(profileAttributeId);
            }

            return profileAttributeIds.ToArray();
        }

        public static ProfileAttributeId[] BuildProfileAttributeIds(DocumentProfileItems documentProfileItems)
        {
            var attributeIds = new List<ProfileAttributeId>();

            if (documentProfileItems == null) return attributeIds.ToArray();

            var profileProperties = documentProfileItems.GetType().GetProperties();

            foreach (var profileProperty in profileProperties)
            {
                ProfileAttributeId profileAttribute;

                if (Enum.TryParse(profileProperty.Name, out profileAttribute))
                {
                    attributeIds.Add(profileAttribute);
                }
            }

            return attributeIds.ToArray();
        }

        public static imProfileAttributeID[] BuildImProfileAttributeIds(DocumentProfileItems documentProfileItems)
        {
            var attributeIds = new List<imProfileAttributeID>();

            if (documentProfileItems == null) return attributeIds.ToArray();

            var profileProperties = documentProfileItems.GetType().GetProperties();

            foreach (var profileProperty in profileProperties)
            {
                imProfileAttributeID profileAttribute;

                if (Enum.TryParse("imProfile" + profileProperty.Name, out profileAttribute))
                {
                    attributeIds.Add(profileAttribute);
                }
            }

            return attributeIds.ToArray();
        }

        public static imProfileAttributeID[] BuildImProfileAttributeIds(ProfileAttributeId[] profileAttributeIds)
        {
            var profileAttributeIdList = new List<imProfileAttributeID>();
            if (profileAttributeIds == null) return profileAttributeIdList.ToArray();

            foreach (var profileAttributeId in profileAttributeIds)
            {
                imProfileAttributeID profileAttribute;

                if (Enum.TryParse("imProfile" + profileAttributeId.ToString(), out profileAttribute))
                {
                    profileAttributeIdList.Add(profileAttribute);
                }
            }

            return profileAttributeIdList.ToArray();
        }

        public static ProfileItem[] BuildProfileItems(DocumentProfileItems documentProfileItems)
        {
            var items = new List<ProfileItem>();

            var profileProperties = documentProfileItems.GetType().GetProperties();
            foreach (var profileProperty in profileProperties)
            {
                imProfileAttributeID profileAttributeId;
                var profilePropertyName = profileProperty.Name;

                if (Enum.TryParse("imProfile" + profileProperty.Name, out profileAttributeId))
                {                  
                    var value = string.Empty;

                    if (profilePropertyName == "Class" || profilePropertyName == "Type")
                    {
                        value = profileProperty.GetValue(documentProfileItems).ToString();
                    }
                    else
                    {
                        var property = documentProfileItems.GetType().GetProperty(profilePropertyName).GetValue(documentProfileItems);
                        if (property == null) continue;

                        value = property.ToString();
                    }

                    if (!string.IsNullOrEmpty(value))
                        items.Add(new ProfileItem() { AttributeID = profileAttributeId, Value = value });
                }
                if (profilePropertyName == "EmailProfileItems")
                {
                    BuildEmailProfileItems(
                        (EmailProfileItems)documentProfileItems.GetType().GetProperty(profilePropertyName).GetValue(documentProfileItems),
                        items);
                }
            }

            #region Custom Profile Attributes Unused
            //items.Add(BuildProfileItem(imProfileAttributeID.imProfileCustom1, DateTime.Now.ToString()));
            //items.Add(BuildProfileItem(imProfileAttributeID.imProfileCustom2, "Custom2"));
            //items.Add(BuildProfileItem(imProfileAttributeID.imProfileCustom5, "Custom5"));
            //items.Add(BuildProfileItem(imProfileAttributeID.imProfileCustom6, "Custom6"));
            //items.Add(BuildProfileItem(imProfileAttributeID.imProfileCustom7, "Custom7"));
            //items.Add(BuildProfileItem(imProfileAttributeID.imProfileCustom8, "Custom8"));
            //items.Add(BuildProfileItem(imProfileAttributeID.imProfileCustom9, "Custom9"));
            //items.Add(BuildProfileItem(imProfileAttributeID.imProfileCustom10, "Custom10"));
            //items.Add(BuildProfileItem(imProfileAttributeID.imProfileCustom23, DateTime.Now.ToString()));
            //items.Add(BuildProfileItem(imProfileAttributeID.imProfileCustom25, "Custom25"));
            //items.Add(BuildProfileItem(imProfileAttributeID.imProfileCustom26, "Custom26"));
            //items.Add(BuildProfileItem(imProfileAttributeID.imProfileCustom27, "Custom27"));
            //items.Add(BuildProfileItem(imProfileAttributeID.imProfileCustom28, "Custom28"));
            //items.Add(BuildProfileItem(imProfileAttributeID.imProfileCustom1Description, "Custom1Desc"));
            //items.Add(BuildProfileItem(imProfileAttributeID.imProfileCustom2Description, "Custom2Desc"));
            //items.Add(BuildProfileItem(imProfileAttributeID.imProfileCustom3Description, "Custom3Desc"));
            //items.Add(BuildProfileItem(imProfileAttributeID.imProfileCustom4Description, "Custom4Desc"));
            //items.Add(BuildProfileItem(imProfileAttributeID.imProfileCustom5Description, "Custom5Desc"));
            //items.Add(BuildProfileItem(imProfileAttributeID.imProfileCustom6Description, "Custom6Desc"));
            //items.Add(BuildProfileItem(imProfileAttributeID.imProfileCustom7Description, "Custom7Desc"));
            //items.Add(BuildProfileItem(imProfileAttributeID.imProfileCustom8Description, "Custom8Desc"));
            //items.Add(BuildProfileItem(imProfileAttributeID.imProfileCustom9Description, "Custom9Desc"));
            //items.Add(BuildProfileItem(imProfileAttributeID.imProfileCustom10Description, "Custom10Desc"));
            //items.Add(BuildProfileItem(imProfileAttributeID.imProfileCustom11Description, "Custom11Desc"));
            //items.Add(BuildProfileItem(imProfileAttributeID.imProfileCustom12Description, "Custom12Desc"));
            #endregion

            return items.ToArray(); 
        }    
        
        private static void BuildEmailProfileItems(EmailProfileItems emailProfileItems, List<ProfileItem> profileItems)
        {
            if (emailProfileItems == null) return;

            var emailProfileProperties = emailProfileItems.GetType().GetProperties();
            foreach (var emailProfileProperty in emailProfileProperties)
            {
                var emailProfilePropertyName = emailProfileProperty.Name;

                EmailFields emailField;
                if (Enum.TryParse(emailProfilePropertyName, out emailField))
                {
                    var emailFieldValue = (int)emailField;
                    var profileAttributeId = (imProfileAttributeID)Enum.Parse(
                        typeof(imProfileAttributeID),
                        "imProfileCustom" + emailFieldValue.ToString()
                        );
                    var value = emailProfileItems.GetType().GetProperty(emailProfilePropertyName)
                        .GetValue(emailProfileItems, null) as string;
                    if (emailProfilePropertyName.ToLower() == "received" | emailProfilePropertyName.ToLower() == "sent")
                    {
                        var dateTime = (DateTime)emailProfileItems.GetType().GetProperty(emailProfilePropertyName)
                        .GetValue(emailProfileItems, null);
                        value = dateTime.ToString();
                    }

                    if (value != null && value != String.Empty)
                        profileItems.Add(new ProfileItem() { AttributeID = profileAttributeId, Value = value });
                }
            }
        }

        public static ProfileItem[] BuildEmailProfileItems(DocumentProfileItems documentProfileItems)
        {
            var profileItems = new List<ProfileItem>();

            BuildEmailProfileItems(documentProfileItems.EmailProfileItems, profileItems);

            return profileItems.ToArray();
        }  
    }
}
