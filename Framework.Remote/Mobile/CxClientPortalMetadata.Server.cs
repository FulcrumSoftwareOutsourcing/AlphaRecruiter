using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Framework.Metadata;
using Framework.Utils;

namespace Framework.Remote.Mobile
{
    public partial class CxClientPortalMetadata
    {
        //----------------------------------------------------------------------------
        public CxClientPortalMetadata(
            CxSlSectionsMetadata sectionsMetadata,
            List<CxClientRowSource> staticRowSources,
            CxAssembliesMetadata assemblies,
            CxClassesMetadata classes,
            CxSlFramesMetadata frames,
            CxImagesMetadata images,
            CxConstraintsMetadata constraints,
            CxSlMetadataHolder holder)
        {
            if (sectionsMetadata == null)
                throw new ArgumentNullException();
            if (sectionsMetadata.Items == null)
                throw new ArgumentNullException();

            sectionsMetadata.RefreshDynamicTreeItems();
            IList<CxSlSectionMetadata> sections = SortSections(new List<CxSlSectionMetadata>(sectionsMetadata.Items));

            foreach (CxSlSectionMetadata section in sections)
            {
                Sections.Add(new CxClientSectionMetadata(section));
            }



            StaticRowsources = staticRowSources;

            if (assemblies != null)
            {
                foreach (CxAssemblyMetadata assembly in assemblies.Assemblies.Values)
                {
                    Assemblies.Add(new CxClientAssemblyMetadata(assembly));
                }
            }

            if (classes != null)
            {
                foreach (CxClassMetadata classMetadata in classes.Classes.Values)
                {
                    Classes.Add(new CxClientClassMetadata(classMetadata));
                }
            }

            foreach (CxSlFrameMetadata frame in frames.AllItems)
            {
                Frames.Add(new CxLayoutElement(frame));
            }

            Images = new CxClientImageMetadata[images.Images.Count];
            int index = 0;
            foreach (CxImageMetadata imageMetadata in images.Images.Values)
            {
                Images[index++] = new CxClientImageMetadata(imageMetadata);
            }

            foreach (CxClientAssemblyMetadata assembly in Assemblies)
            {
                if (!string.IsNullOrEmpty(assembly.SlPluginPath))
                {
                    AssembliesData.Add(assembly.Id, GetAssemblyBytes(assembly));
                }
            }

            ClientEntityMarks = CxClientEntityMarks.Greate();

            foreach (string key in constraints.Constraints)
            {
                this.Constraints.Add(key, constraints.Constraints[key]);
            }

            JsAppProperties = sectionsMetadata.JsAppProperties;

        }

        //----------------------------------------------------------------------------
        /// <summary>
        /// Sorting sections according to the Display Order.
        /// The sections without that value should go at last.
        /// </summary>
        /// <param name="sections">list of sections to sort</param>
        /// <returns>sorted list</returns>
        public IList<CxSlSectionMetadata> SortSections(IList<CxSlSectionMetadata> sections)
        {
            List<CxSlSectionMetadata> sectionsToSort = new List<CxSlSectionMetadata>();
            List<CxSlSectionMetadata> sectionsToNotSort = new List<CxSlSectionMetadata>();
            foreach (CxSlSectionMetadata section in sections)
            {
                if (section.DisplayOrder == int.MaxValue)
                    sectionsToNotSort.Add(section);
                else
                    sectionsToSort.Add(section);
                if (section.Items != null && section.Items.Items != null)
                {
                    List<CxSlTreeItemMetadata> children = new List<CxSlTreeItemMetadata>(section.Items.Items);
                    section.Items.Items.Clear();
                    foreach (CxSlTreeItemMetadata child in SortTreeItems(children))
                    {
                        section.Items.Items.Add(child);
                    }
                }
            }
            sectionsToSort.Sort(((a1, a2) => a1.DisplayOrder - a2.DisplayOrder));

            List<CxSlSectionMetadata> result = new List<CxSlSectionMetadata>(sectionsToSort);
            result.AddRange(sectionsToNotSort);
            return result;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Sorting tree items according to the Display Order.
        /// The sections without that value should go at last.
        /// </summary>
        /// <param name="items">list of tree items to sort</param>
        /// <returns>sorted list</returns>
        public IList<CxSlTreeItemMetadata> SortTreeItems(IList<CxSlTreeItemMetadata> items)
        {
            List<CxSlTreeItemMetadata> itemsToSort = new List<CxSlTreeItemMetadata>();
            List<CxSlTreeItemMetadata> itemsToNotSort = new List<CxSlTreeItemMetadata>();
            foreach (CxSlTreeItemMetadata item in items)
            {
                if (item.DisplayOrder == int.MaxValue)
                    itemsToNotSort.Add(item);
                else
                    itemsToSort.Add(item);
                if (item.Items != null && item.Items.Items != null)
                {
                    List<CxSlTreeItemMetadata> children = new List<CxSlTreeItemMetadata>(item.Items.Items);
                    item.Items.Items.Clear();
                    foreach (CxSlTreeItemMetadata child in SortTreeItems(children))
                    {
                        item.Items.Items.Add(child);
                    }
                }
            }
            itemsToSort.Sort(((a1, a2) => a1.DisplayOrder - a2.DisplayOrder));

            List<CxSlTreeItemMetadata> result = new List<CxSlTreeItemMetadata>(itemsToSort);
            result.AddRange(itemsToNotSort);
            return result;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Returns Silverlight plugin assembly as array of bytes. 
        /// </summary>
        /// <param name="clientAssemblyMeta">CxAssemblyMetadata to load bytes.</param>
        /// <returns>Silverlight plugin assembly as array of bytes. </returns>
        public byte[] GetAssemblyBytes(CxClientAssemblyMetadata clientAssemblyMeta)
        {

            string pluginFolder = clientAssemblyMeta.SlPluginPath;
            if (string.IsNullOrEmpty(pluginFolder))
            {
                throw new ExException(
                  string.Format("'sl_plugin_path' attribute is not defined for assembly with id '{0}.'", clientAssemblyMeta.Id));
            }

            string pluginPath = HttpContext.Current.Server.MapPath(
              Path.Combine("SlClientAssemblies", pluginFolder));
            string pluginFullPath = Path.Combine(pluginPath, Path.GetFileName(clientAssemblyMeta.FileName));
            if (!File.Exists(pluginFullPath))
            {
                throw new ExException(
                  string.Format("Plugin client assembly '{0}' does not exists.", pluginFullPath));
            }

            byte[] asmData;
            using (FileStream fs = new FileStream(pluginFullPath, FileMode.Open, FileAccess.Read))
            {
                asmData = new byte[fs.Length];
                fs.Read(asmData, 0, asmData.Length);
            }
            return asmData;

        }
    }
}
