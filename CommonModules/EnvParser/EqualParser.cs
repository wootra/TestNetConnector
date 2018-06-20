using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using DataHandling;
namespace EnvParsers
{
    public class EqualParser
    {
        public static void getEnv(String filePath, ref Dictionary<String,String> env)
        {

            String[] info = null;
            
            try
            {
                info = File.ReadAllLines(filePath);
            }
            catch (Exception ex)
            {
                throw new Exception(filePath + " doesn't exist..\n" + ex.Message);
            }

            for (int i = 0; i < info.Length; i++) getAEnvLine(info[i], env);
        }

        static void getAEnvLine(String aLine, Dictionary<String,String> env)
        {
            if (aLine.Trim().Length == 0 || aLine.Trim()[0] == '#') return;

            String[] var = aLine.Split("=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (var.Count() > 1)
            {
                env[var[0].Trim()] = var[1].Trim(" \t;\"".ToCharArray());
            }
        }
        public static CustomDictionary<String, String> getEnv(String filePath)
        {
            CustomDictionary<String, String> dic = new CustomDictionary<string, string>();
            String[] info = null;
            try
            {
                info = File.ReadAllLines(filePath);
            }
            catch (Exception ex)
            {
                
                throw new Exception(filePath + " doesn't exist..\n" + ex.Message);
                
            }
            for (int i = 0; i < info.Length; i++)
            {
                KeyValuePair<String, String> aEnv = getAEnvLine(info[i]);
                if (aEnv.Key!=null) dic.Add(aEnv);
            }

            return dic;
        }

        static KeyValuePair<String, String> getAEnvLine(String aLine)
        {
            if (aLine.Trim().Length == 0 || aLine.Trim()[0] == '#') return new KeyValuePair<string, string>(null, null);

            String[] var = aLine.Split("=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (var.Length > 1)
            {
                return new KeyValuePair<string, string>(var[0].Trim(), var[1].Trim(" \t;\"".ToCharArray()));
            }
            else return new KeyValuePair<string, string>(null, null);
        }

        public static void setEnv(String filePath, Dictionary<String,String> env, bool makeNewFileIfNotExists=true){
            if (File.Exists(filePath) == false)
            {
                if(makeNewFileIfNotExists) makeNewEnv(filePath, env);
            }
            String[] info = null;

            try
            {
                info = File.ReadAllLines(filePath);//원래의 구조를 읽어들인다.
            }
            catch (Exception ex)
            {
                throw new Exception(filePath + " doesn't exist..\n" + ex.Message);
            }
            
            for (int i = 0; i < info.Length; i++)
            {
                if (info[i].Trim().Length == 0 || info[i].Trim()[0] == '#')
                {
                 
                }
                else
                {

                    String[] var = info[i].Split("=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    if (var.Count() > 1)
                    {
                        String key = var[0].Trim();
                        if (env.Keys.Contains(key))
                        {
                            info[i] = key + " = " + env[key];
                        }
                    }
                }
            
            }

            File.WriteAllLines(filePath, info);
            
        }

        public static void makeNewEnv(String filePath, Dictionary<String, String> env)
        {

            List<String> infos = new List<string>();
            for (int i = 0; i < env.Keys.Count; i++)
            {
                infos.Add(env.Keys.ElementAt(i) + " = " + env.Values.ElementAt(i));
            }
            
            File.WriteAllLines(filePath, infos.ToArray());

        }
        
    }
}
