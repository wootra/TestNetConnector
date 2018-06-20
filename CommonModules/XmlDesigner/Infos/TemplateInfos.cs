using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TestNgineData.PacketDatas;
using System.Xml;
using XmlHandlers;

namespace XmlDesigner
{
    public class TemplateInfos
    {
        public static TemplateInfos This;
        /*
        public Dictionary<String, IPacketBase> IOCMD = new Dictionary<string, IPacketBase>();
        public Dictionary<String, Image> IconIO = new Dictionary<string, Image>();
        public Dictionary<String, IPacketBase> LCMD = new Dictionary<string, IPacketBase>();
        public Dictionary<String, Image> IconLogical = new Dictionary<string, Image>();
        public Dictionary<String, IPacketBase> M1553CMD = new Dictionary<string, IPacketBase>();
        public Dictionary<String, Image> IconM1553 = new Dictionary<string, Image>();
        public Dictionary<String, IPacketBase> CUSTOM = new Dictionary<string, IPacketBase>();
        public Dictionary<String, Image> IconCustom = new Dictionary<string, Image>();
        public Dictionary<String, IPacketBase> TOTALCMD = new Dictionary<string, IPacketBase>();
        public Dictionary<String, Image> IconTotal = new Dictionary<string, Image>();
        */
        Dictionary<String, XmlTemplate> _totalTemplates = new Dictionary<string, XmlTemplate>();
        public Dictionary<String, String> TotalIcons = new Dictionary<string, String>();
        Dictionary<String, XmlTemplateGroup> _totalTemplateGroup = new Dictionary<string, XmlTemplateGroup>();

        //public List<String> Cmds = new List<string>();
        public Dictionary<String, Dictionary<String, String>> Icons = new Dictionary<string, Dictionary<string, String>>();
        public Dictionary<String, String> TemplateBaseIcons = new Dictionary<string, String>();

        String _projectBase = "../TestNgineData/Projects";//
        String _scenarioDir = "";

        String _commonTemplateDir = "../TestNgineData/Templates";
        String _commonUITemplateDir = "../TestNgineData/UITemplates";

        public TemplateInfos()
        {
            LoadTemplates();
            This = this;
        }

        /// <summary>
        /// 저장된 Template을 가져온다. 없을 시 null을 리턴한다.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public XmlTemplate GetTemplate(String name, bool ignoreCase=true)
        {
            if(name.Contains(".")){
                name = name.Substring(name.LastIndexOf(".")+1);
            }
            if (_totalTemplates.ContainsKey(name))
            {
                return _totalTemplates[name];
            }
            else if (ignoreCase)
            {
                foreach (String key in _totalTemplates.Keys)
                {
                    if (key.ToLower().Equals(name.ToLower())) return _totalTemplates[key];
                }
                return null;
            }
            else return null;
        }

        public XmlTemplateGroup GetTemplateGroup(string name)
        {
            if (name.Contains("."))
            {
                name = name.Substring(name.LastIndexOf(".") + 1);
            }
            if (_totalTemplateGroup.ContainsKey(name)) return _totalTemplateGroup[name];
            else return null;
        }



        Dictionary<String, String> _templatesPath = new Dictionary<string, string>();
        String _projectPath;
        public void LoadTemplates()
        {
            //_commonTemplateDir = Directory.GetCurrentDirectory() + "\\" + _commonTemplateDir;
            
            /*
            if (Directory.Exists(_commonTemplateDir) == false) Directory.CreateDirectory(_commonTemplateDir);

            _projectPath = Directory.GetCurrentDirectory() + "\\" + _projectBase + "\\" + projectName;
            _scenarioDir = _projectPath + "\\Scenarios";
            */
            String[] subTemplates = Directory.GetDirectories(_commonTemplateDir);
            for (int si = 0; si < subTemplates.Length; si++)
            {
                String subName = subTemplates[si].Substring(subTemplates[si].LastIndexOf("\\") + 1);

                //Cmds.Add(subName);
                //Dictionary<string, XmlTemplate> cmds = Cmds[subName] = new Dictionary<string, XmlTemplate>();
                Dictionary<String, String> icons = Icons[subName] =  new Dictionary<string, String>();

                String templateBaseIconPath = Path.GetFullPath((_commonTemplateDir+"\\"+subName+".png").Replace("/","\\"));

                if (File.Exists(templateBaseIconPath)) TemplateBaseIcons[subName] = templateBaseIconPath;// new BitmapImage(new Uri(templateBaseIconPath));

                String[] filePath = Directory.GetFiles(subTemplates[si]);
                
                for (int i = 0; i < filePath.Length; i++)
                {
                    //Con_Peer peer = new Con_Peer(peerPaths[i]);
                    String fileName = filePath[i].Substring(filePath[i].LastIndexOf("\\") + 1);
                    
                    string ext = fileName.Substring(fileName.LastIndexOf(".") + 1).ToLower(); //확장자..
                    String name = fileName.Substring(0, fileName.LastIndexOf("."));// 확장자를 뗌..
                    //String absName = name;// subName + "." + name;

                    if (ext.Equals("xml"))
                    {
                        XmlDocument xDoc;
                        XmlNode rootNode = XmlGetter.RootNode(out xDoc, filePath[i]);
                        if (rootNode.Name.ToLower().Equals("packet"))
                        {

                            _templatesPath[name] = filePath[i].Replace("/", "\\");

                            XmlTemplate template = new XmlTemplate(name);
                            template.LoadXml(_templatesPath[name]);

                            _totalTemplates.Add(name, template);
                            if (icons.ContainsKey(name)) //icon이 이전에 설정되었으면
                                template.ImagePath = icons[name];
                            else
                                icons[name] = null; //key를 사용할 것이므로 일단 null이라도 넣어서 방을 만든다.
                            //cmds.Add(name, template);
                        }
                        else if (rootNode.Name.ToLower().Equals("packetgroup"))
                        {

                        }
                    }
                    else if (ext.Equals("png") || ext.Equals("jpg") || ext.Equals("gif") || ext.Equals("jpeg"))
                    {
                        String fullPath = Path.GetFullPath(filePath[i].Replace("/", "\\"));

                        //BitmapImage img = new BitmapImage(new Uri(fullPath));
                        //icons.Add(name, img);
                        icons[name] = fullPath;//path일단 저장
                        if (_totalTemplates.ContainsKey(name)) //template이 먼저 추가되면
                        {
                            _totalTemplates[name].ImagePath = fullPath; //이미지 설정
                            
                        }
                    }
                }
            }
            /*
            String[] templatePath = Directory.GetFiles(_commonTemplateDir+"\\IOCMD");
            for (int i = 0; i < templatePath.Length; i++)
            {
                //Con_Peer peer = new Con_Peer(peerPaths[i]);
                String name = templatePath[i].Substring(templatePath[i].LastIndexOf("\\") + 1);
                name = name.Substring(0, name.LastIndexOf("."));// .xml을 떼어줌..
                _templatesPath[name] = templatePath[i].Replace("/", "\\");
                XmlTemplate template = new XmlTemplate();
                template.LoadXml(_templatesPath[name]);
                TOTALCMD.Add(name, template);
                IOCMD.Add(name, template);
            }
            templatePath = Directory.GetFiles(_commonTemplateDir + "\\LCMD");
            for (int i = 0; i < templatePath.Length; i++)
            {
                //Con_Peer peer = new Con_Peer(peerPaths[i]);
                String name = templatePath[i].Substring(templatePath[i].LastIndexOf("\\") + 1);
                name = name.Substring(0, name.LastIndexOf("."));// .xml을 떼어줌..
                _templatesPath[name] = templatePath[i].Replace("/", "\\");
                XmlTemplate template = new XmlTemplate();
                template.LoadXml(_templatesPath[name]);
                TOTALCMD.Add(name, template);
                LCMD.Add(name, template);
            }
            templatePath = Directory.GetFiles(_commonTemplateDir + "\\M1553CMD");
            for (int i = 0; i < templatePath.Length; i++)
            {
                //Con_Peer peer = new Con_Peer(peerPaths[i]);
                String name = templatePath[i].Substring(templatePath[i].LastIndexOf("\\") + 1);
                name = name.Substring(0, name.LastIndexOf("."));// .xml을 떼어줌..
                _templatesPath[name] = templatePath[i].Replace("/", "\\");
                XmlTemplate template = new XmlTemplate();
                template.LoadXml(_templatesPath[name]);
                TOTALCMD.Add(name, template);
                M1553CMD.Add(name, template);
            }
             

            templatePath = Directory.GetFiles(_projectPath + "/Templates");

            for (int i = 0; i < templatePath.Length; i++)
            {
                //Con_Peer peer = new Con_Peer(peerPaths[i]);
                String name = templatePath[i].Substring(templatePath[i].LastIndexOf("\\") + 1);
                name = name.Substring(0, name.LastIndexOf("."));// .xml을 떼어줌..
                _templatesPath[name] = templatePath[i].Replace("/", "\\");
                //node.RelativeObject["peer"] = peer;
                //peer.SetName(name);
                //_activatedPeerNode = node;


                //_peersGroup[peer] = name;//마지막 읽은 group을 _activatedPeer에 배정한다.
                //peer.SetMsgList(name, new ConMsgList(name));
            }
             */
        }
    }
}
