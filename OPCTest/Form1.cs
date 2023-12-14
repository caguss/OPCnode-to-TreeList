using Opc.Ua;
using Opc.UaFx;
using Opc.UaFx.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OPCTest
{
    public partial class Form1 : Form
    {

        Opc.UaFx.Client.OpcClient _pOpcClient = new OpcClient();




        public Form1()
        {
            InitializeComponent();

            try
            {

                _pOpcClient = new Opc.UaFx.Client.OpcClient("opc.tcp://"+ "ip:port");

                //Opc.UaFx.Client.OpcClient opcClient = new Opc.UaFx.Client.OpcClient("opc.tcp://192.168.0.241:56000");
                _pOpcClient.Security.AutoUpgradeEndpointPolicy = false;
                _pOpcClient.Security.AutoAcceptUntrustedCertificates = true;
                _pOpcClient.Configuration.ApplicationUri = "test";

                _pOpcClient.ReconnectTimeout = 2000;//재연결 타임아웃

                _pOpcClient.OperationTimeout = 2000;//연결 타임아웃

                _pOpcClient.DisconnectTimeout = 2000;


                #region ○ 보안정책 ( Security Policy )
                /*
                Aes128_Sha256_RsaOaep (not in .NET Framework, COM or Excel, i.e. only in .NET Standard)
                http://opcfoundation.org/UA/SecurityPolicy#Aes128_Sha256_RsaOaep

                Aes256_Sha256_RsaPss (not in .NET Framework, COM or Excel, i.e. only in .NET Standard)
                http://opcfoundation.org/UA/SecurityPolicy#Aes256_Sha256_RsaPss

                Basic128Rsa15 (obsolete)
                http://opcfoundation.org/UA/SecurityPolicy#Basic128Rsa15

                Basic256	
                http://opcfoundation.org/UA/SecurityPolicy#Basic256

                Basic256Sha256	
                http://opcfoundation.org/UA/SecurityPolicy#Basic256Sha256
                 
                None	
                http://opcfoundation.org/UA/SecurityPolicy#None
                 */
                #endregion

                if (_pOpcClient.State.ToString() != "Connected" && _pOpcClient.State.ToString() != "Reconnected" && _pOpcClient.State.ToString() != "Reconnecting" && _pOpcClient.State.ToString() != "Connecting")
                {
                    _pOpcClient.Connect();
                }
            }
            catch (Exception ex)
            {

            }

            TreenodeSetting();
        }

        private void TreenodeSetting()
        {
            try
            {

                OpcBrowseNode obn = new OpcBrowseNode(OpcObjectTypes.ObjectsFolder, OpcBrowseNodeDegree.Generation,
                    referenceTypes: new[] {
                            OpcReferenceType.Organizes,
                            OpcReferenceType.HasComponent,
                            OpcReferenceType.HasProperty,

                    });

                var node = _pOpcClient.BrowseNode(obn);
                TreeNode tn = OpcUaFindTreeNodes(node);
                NodesTree.Nodes.Add(tn);

            }
            catch (Exception ex)
            {
            }
        }

        #region ○ 전체노드 찾기
        private TreeNode OpcUaFindTreeNodes(OpcNodeInfo node)
        {
            TreeNode parentTree = (new TreeNode(node.Name.ToString()));
            NodesTree.Nodes.Add(parentTree);

            foreach (var childNode in node.Children())
            {
                TreeNode childTree = new TreeNode();
                childTree.Text = childNode.Name.ToString();

                if (childNode.Children().Count() > 0)
                {
                    ReqNode(childNode, childTree);
                }

                NodesTree.Nodes.Add(childTree);
            }
            return parentTree;
        }

        private static void ReqNode(OpcNodeInfo childNode, TreeNode childTree)
        {
            foreach (var childNode2 in childNode.Children())
            {
                TreeNode childTree2 = new TreeNode();
                childTree2.Text = childNode2.Name.ToString();
                childTree.Nodes.Add(childTree2);

                if (childNode2.Children().Count() > 0)
                {
                    ReqNode(childNode2, childTree2);
                }
            }
        }
        #endregion

      
    }
}
