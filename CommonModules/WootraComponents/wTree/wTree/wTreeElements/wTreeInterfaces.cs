using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WootraComs.wTreeElements
{
    public interface IwTreeNodeCollectionParent
    {
        /// <summary>
        /// treeNode의 depth를 가리킨다. 최상위일 경우 0이다.
        /// </summary>
        int Depth { get; }

        /// <summary>
        /// treeNode의 부모를 가리킨다. 최상위일 경우 null이다.
        /// </summary>
        IwTreeNodeCollectionParent TreeParent { get; }
    }
}
