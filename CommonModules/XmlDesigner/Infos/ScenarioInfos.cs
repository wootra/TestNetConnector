using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TestNgineData.PacketDatas;

namespace XmlDesigner
{
    public class ScenarioInfos : Dictionary<String, XmlScenario>
    {
        public static ScenarioInfos This;
        
        String _projectBase = "../TestNgineData/Projects";//
        string _projectPath;

        //String _targetTable = "
        //TriStateTreeNode _activatedPeerNode;
        //ConMsgList _activatedMsgList;

        //List<Con_Peer> _peers = new List<Con_Peer>();

        String _scenarioDir = "";
        String[] _scenarioPaths;
        

        public ScenarioInfos(String projectName)
        {
            This = this;
            LoadScenarios(projectName);
        }

        public void LoadScenarios(string projectName)
        {
            //this.ClearAllItems();

            _projectPath = Directory.GetCurrentDirectory() + "\\" + _projectBase + "\\" + projectName;
            _scenarioDir = _projectPath + "\\Scenarios";


            if (Directory.Exists(_scenarioDir) == false) Directory.CreateDirectory(_scenarioDir);
            _scenarioPaths = Directory.GetDirectories(_scenarioDir);

            
            String[] scenarioPath = Directory.GetFiles(_scenarioDir);
            this.Clear();

            //this.AddContextMenuItemEndNode(_groupsRoot, "새 그룹 생성");

            //try
            {
                for (int i = 0; i < scenarioPath.Length; i++)
                {
                    string path = scenarioPath[i].Replace("/", "\\");
                    String fileName = scenarioPath[i].Substring(path.LastIndexOf("\\") + 1);
                    String name = fileName.Substring(0, fileName.LastIndexOf("."));
                    this.Add(name, LoadScenario(path));
                }
            }
            //catch (Exception e)
            {
                //throw e;
            }
        }

        
        XmlScenario LoadScenario(String scenarioPath)
        {
            XmlScenario xScenario = new XmlScenario();
            try
            {
                xScenario.LoadXml(scenarioPath);
            }
            catch (Exception e)
            {
                throw new Exception("Error on Loading Scenario["+scenarioPath + "]..\r\n" + e.Message);
            }
            return xScenario;
        }
    }
}
