using System;
using System.Collections.Specialized;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Resources.Media;
using Sitecore.SecurityModel;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Web.UI.Sheer;

namespace ImageFocalPointPicker.Fields
{
    public class ImageFocalPointPickerField : Image
    {
        public override void HandleMessage(Message message)
        {
            if (message["id"] != this.ID)
            {
                base.HandleMessage(message);
            }

            string name = message.Name;

            if (name == "contentimage:focalpoint")
            {
                string onFocus = this.Attributes["onfocus"];

                Sitecore.Context.ClientPage.Start(
                    this,
                    "PickFocalPoint",
                    new NameValueCollection
                    {
                        {"containerId", onFocus.Split('?')[0].Split('{')[1].Replace("}", string.Empty)},
                        {
                            "fieldId",
                            onFocus.Substring(onFocus.IndexOf("fld", StringComparison.Ordinal) + 5).Substring(0, 36)
                        }
                    });
            }
            else
            {
                base.HandleMessage(message);
            }
        }

        protected void PickFocalPoint(ClientPipelineArgs args)
        {
            if (args.IsPostBack)
            {
                if (args.HasResult && args.Result != this.Value)
                {
                    this.SetModified();
                    string attribute = this.XmlValue.GetAttribute("mediaid");
                    if (string.IsNullOrEmpty(attribute))
                    {
                        SheerResponse.Alert("Select an image from the Media Library first.");
                    }
                    Item obj = Client.ContentDatabase.GetItem(attribute);
                    obj.Editing.BeginEdit();
                    obj["description"] = args.Result;
                    obj.Editing.EndEdit();
                }
            }
            else
            {
                string attribute = this.XmlValue.GetAttribute("mediaid");
                if (string.IsNullOrEmpty(attribute))
                {
                    SheerResponse.Alert("Select an image from the Media Library first.");
                }
                else
                {
                    Item obj = Client.ContentDatabase.GetItem(attribute);
                    if (obj == null)
                        return;

                    // Optional check to display number of instances this image is used - since the update is being made to media item

                    /* ItemLink[] referrers = Globals.LinkDatabase.GetReferrers(obj);
                    if (referrers != null && referrers.Length > 1)
                    {
                        SheerResponse.Confirm(
                            $"This media item is referenced by {(object)referrers.Length} other items.\n\nEditing the media item will change it for all the referencing items.\n\nAre you sure you want to continue?");
                        args.WaitForPostBack();
                        return;
                    } */

                    string uri =
                        $"{Sitecore.UIUtil.GetUri("control:ImageFocalPointPickerDialog")}&value={obj["Description"]}&containerId={args.Parameters["containerId"]}&fieldId={args.Parameters["fieldId"]}";
                    SheerResponse.ShowModalDialog(uri, "500", "500", string.Empty, true);
                    args.WaitForPostBack();
                }
            }
        }
    }
}