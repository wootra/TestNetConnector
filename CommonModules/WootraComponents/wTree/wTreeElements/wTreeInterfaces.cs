using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WootraComs.wTreeElements
{
    /// <summary>
    /// wTreeNode의 부모가 될 수 있는 Item이다. 단, null이 될 수 없으며 최상단에는 wTree가 존재한다.
    /// </summary>
    public interface IwTreeNodeCollectionParent
    {
        /// <summary>
        /// treeNode의 depth를 가리킨다. 최상위일 경우 0이다.
        /// </summary>
        int Depth { get; }

        /// <summary>
        /// treeNode의 부모를 가리킨다. 최상위일 경우 wTree이다.
        /// </summary>
        IwTreeNodeCollectionParent TreeParent { get; }

        /// <summary>
        /// Node에 속해있는 자식노드 Collection.
        /// </summary>
        wTreeNodeCollection Children{get;}

        bool IsExpanded { get; set; }

        /// <summary>
        /// 노드의 이름..
        /// </summary>
        String Name { get; set; }
    }
}
