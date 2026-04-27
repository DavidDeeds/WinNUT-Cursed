#nullable enable
using System.Text;
using WinNUT_Client.Properties;
using WinNUT_Client_Common;

namespace WinNUT_Client;

public partial class ListVarGui : Form
{
    private List<UPS_List_Datas>? _listVarDatas;
    private readonly UpsDevice _upsDevice;
    private readonly string _upsName;

    public ListVarGui(UpsDevice? device)
    {
        _upsDevice = device ?? throw new ArgumentNullException(nameof(device));
        _upsName = _upsDevice.Nut_Config.UPSName;
        InitializeComponent();
        Load += ListVarGui_Load;
        Timer_Update_List.Tick += Event_Update_List;
        Btn_Close.Click += Btn_Close_Click;
        Btn_Reload.Click += Btn_Reload_Click;
        TView_UPSVar.AfterSelect += TView_UPSVar_AfterSelect;
        Btn_Clip.Click += Btn_Clip_Click;
        Btn_Save.Click += Btn_Save_Click;
    }

    private void ListVarGui_Load(object? sender, EventArgs e)
    {
        WinNutGlobals.LogFile.LogTracing("Load List Var Gui", LogLvl.LOG_DEBUG, this);
        foreach (Form f in Application.OpenForms)
        {
            if (f is WinNUT main)
            {
                Icon = main.Icon;
                break;
            }
        }

        Visible = false;
        PopulateTreeView();
        Visible = true;
    }

    private void PopulateTreeView()
    {
        WinNutGlobals.LogFile.LogTracing("Populate TreeView", LogLvl.LOG_DEBUG, this);
        try
        {
            _upsDevice.IsUpdatingData = false;
            _listVarDatas = _upsDevice.GetUPS_ListVar();
            _upsDevice.IsUpdatingData = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error encountered trying to get variables from the UPS: " + Environment.NewLine + ex.Message,
                "Error Encountered");
            return;
        }

        if (_listVarDatas == null)
        {
            WinNutGlobals.LogFile.LogTracing("ListUPSVars return Nothing Value", LogLvl.LOG_DEBUG, this);
            return;
        }

        TView_UPSVar.Nodes.Clear();
        var ups = Properties.Settings.Default.NUT_UPSName;
        TView_UPSVar.Nodes.Add(ups, ups);
        var root = TView_UPSVar.Nodes[0];
        foreach (var upsVar in _listVarDatas)
        {
            var current = root;
            var fullPathNode = string.Empty;
            foreach (var subPath in upsVar.VarKey.Split('.'))
            {
                fullPathNode += subPath + ".";
                var found = TView_UPSVar.Nodes.Find(fullPathNode, true);
                if (found.Length == 0)
                {
                    current = current.Nodes.Add(fullPathNode, subPath);
                }
                else
                {
                    current = found[0];
                }
            }
        }
    }

    private void Event_Update_List(object? sender, EventArgs e)
    {
        var selectedNode = TView_UPSVar.SelectedNode;
        if (selectedNode?.Parent == null)
        {
            return;
        }

        if (selectedNode.Parent.Text != _upsName && selectedNode.Nodes.Count == 0)
        {
            var varName = selectedNode.FullPath.Replace(_upsName + ".", "");
            WinNutGlobals.LogFile.LogTracing("Update {VarName}", LogLvl.LOG_DEBUG, this);
            Lbl_V_Value.Text = _upsDevice.GetUPSVar(varName);
        }
    }

    private void Btn_Close_Click(object? sender, EventArgs e)
    {
        WinNutGlobals.LogFile.LogTracing("Close List Var Gui", LogLvl.LOG_DEBUG, this);
        Close();
    }

    private void Btn_Reload_Click(object? sender, EventArgs e)
    {
        WinNutGlobals.LogFile.LogTracing("Reload Treeview from Button", LogLvl.LOG_DEBUG, this);
        Lbl_N_Value.Text = "";
        Lbl_V_Value.Text = "";
        Lbl_D_Value.Text = "";
        TView_UPSVar.Nodes.Clear();
        PopulateTreeView();
    }

    private void TView_UPSVar_AfterSelect(object? sender, TreeViewEventArgs e)
    {
        var upsName = Properties.Settings.Default.NUT_UPSName;
        var fp = e.Node.FullPath;
        var selectedChild = fp.StartsWith(upsName + ".", StringComparison.Ordinal)
            ? fp.Substring(upsName.Length + 1)
            : fp;

        var foundIndex = _listVarDatas?.FindIndex(x => x.VarKey == selectedChild) ?? -1;

        if (selectedChild != upsName && foundIndex >= 0 && _listVarDatas != null)
        {
            var item = _listVarDatas[foundIndex];
            WinNutGlobals.LogFile.LogTracing("Select " + item.VarKey + " Node", LogLvl.LOG_DEBUG, this);
            Lbl_N_Value.Text = item.VarKey;
            Lbl_V_Value.Text = item.VarValue;
            Lbl_D_Value.Text = item.VarDesc;
        }
        else
        {
            Lbl_N_Value.Text = "";
            Lbl_V_Value.Text = "";
            Lbl_D_Value.Text = "";
        }
    }

    private string SerializeUpsData()
    {
        WinNutGlobals.LogFile.LogTracing("Serializing UPS data to String.", LogLvl.LOG_DEBUG, this);
        var sb = new StringBuilder();
        var d = _upsDevice.UPS_Datas;
        sb.AppendLine(_upsDevice.Name + " (" + d.Mfr + "/" + d.Model + "/" + d.Firmware + ")");
        if (_listVarDatas != null)
        {
            foreach (var l in _listVarDatas)
            {
                sb.AppendLine(l.VarKey + " (" + l.VarDesc + ") : " + l.VarValue);
            }
        }

        WinNutGlobals.LogFile.LogTracing("Successfully built serialized string, length: " + sb.Length, LogLvl.LOG_DEBUG,
            this);
        return sb.ToString();
    }

    private void Btn_Clip_Click(object? sender, EventArgs e)
    {
        WinNutGlobals.LogFile.LogTracing("Copy TreeView To Clipboard", LogLvl.LOG_DEBUG, this);
        try
        {
            Clipboard.SetText(SerializeUpsData());
            WinNutGlobals.LogFile.LogTracing("Successfully copied UPS information to the Clipboard.", LogLvl.LOG_NOTICE,
                this, Resources.List_Var_Gui__SetCpbTextSuccess);
        }
        catch (Exception ex)
        {
            var frmtdError = string.Format(Resources.List_Var_Gui__SetCpbTextError_Text, ex.Message);
            WinNutGlobals.LogFile.LogTracing("Exception encountered while attempting to set Clipboard text.",
                LogLvl.LOG_ERROR, this, frmtdError);
            WinNutGlobals.LogFile.LogException(ex, this);
            MessageBox.Show(frmtdError, Resources.List_Var_Gui__SetCpbTextError_Caption, MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void Btn_Save_Click(object? sender, EventArgs e)
    {
        WinNutGlobals.LogFile.LogTracing("Export TreeView To File", LogLvl.LOG_DEBUG, this);
        using var sfd = new SaveFileDialog
        {
            Filter = "Text files|*.txt|All files|*.*",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            Title = Resources.List_Var_Gui__SaveFile_Caption
        };

        if (sfd.ShowDialog() != DialogResult.OK || string.IsNullOrEmpty(sfd.FileName))
        {
            WinNutGlobals.LogFile.LogTracing("SaveFileDialog was not accepted.", LogLvl.LOG_NOTICE, this);
            return;
        }

        WinNutGlobals.LogFile.LogTracing("User completed SaveFileDialog, path: " + sfd.FileName, LogLvl.LOG_NOTICE, this);
        try
        {
            using (var sw = new StreamWriter(sfd.OpenFile()))
            {
                sw.Write(SerializeUpsData());
            }

            WinNutGlobals.LogFile.LogTracing("File saved successfully.", LogLvl.LOG_NOTICE, this,
                string.Format(Resources.List_Var_Gui__SaveFileSuccess, sfd.FileName));
        }
        catch (Exception ex)
        {
            var frmtdError = string.Format(Resources.List_Var_Gui__SaveFileError_Text, ex.Message);
            WinNutGlobals.LogFile.LogTracing("Exception encountered while saving UPS data to a file.", LogLvl.LOG_ERROR,
                this, frmtdError);
            WinNutGlobals.LogFile.LogException(ex, this);
            MessageBox.Show(frmtdError, Resources.List_Var_Gui__SaveFile_Caption, MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
