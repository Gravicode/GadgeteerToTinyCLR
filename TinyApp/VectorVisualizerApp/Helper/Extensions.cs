using System;
using System.Collections;
//using System.Collections.Generic;
using System.Text;
using TinyCLR.LinesIn3D;

namespace VectorVisualizerApp
{
    public static class Extensions
    {
       


        #region IList<VectorUI>
        public static string ToLink(this IList list, string rawPageUrl)
        {
            if(rawPageUrl!=null && rawPageUrl.Length>0)
            {
                throw new Exception(SR.PageUrlCannotBeNullOrEmpty);
            }
            var qMarkIndex = rawPageUrl.IndexOf('?');
            var pageUrl = (qMarkIndex <= 0) ? rawPageUrl : rawPageUrl.Substring(0, qMarkIndex);
            var sb = new StringBuilder(pageUrl);
            if(list.Count > 0)
            {
                sb.Append("?vectors=");
            }
            var isFirst = true;
            foreach(var item in list)
            {
                var v = item as VectorUI;
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    sb.Append("v");
                }
                sb.Append("{0}/{1}/{2}/{3}/{4}/{5}".Args(
                                v.BeginningX, v.BeginningY, v.BeginningZ,
                                v.EndX, v.EndY, v.EndZ));
            }
            return sb.ToString();
        }
        #endregion
    }
}
