#nullable enable
using WinNUT_Client.Models;
using WinNUT_Client.Properties;
using WinNUT_Client_Common.OldParams;

namespace WinNUT_Client;

public partial class UpgradePrefsDialog : Form
{
    private UpgradePrefsDialogModel? _backingDataModel;

    public UpgradePrefsDialog()
    {
        InitializeComponent();
        Load += UpgradePrefsDialog_Load;
        OK_Button.Click += OK_Button_Click;
        Cancel_Button.Click += Cancel_Button_Click;
    }

    private void UpgradePrefsDialog_Load(object? sender, EventArgs e)
    {
        if (!WinNUT_Params.ParamsExist)
        {
            MessageBox.Show(Resources.UpgradePrefsDialog_NoPrefsExistError,
                Resources.UpgradePrefsDialog_NoPrefsExistCaption);
            Close();
            return;
        }

        _backingDataModel = new UpgradePrefsDialogModel(this);
        UpgradePrefsDialogModelBindingSource.DataSource = _backingDataModel;
        _backingDataModel.InitializeProperties();
    }

    private void OK_Button_Click(object? sender, EventArgs e) => _backingDataModel?.BeginUpgradeWorkAsync();

    private void Cancel_Button_Click(object? sender, EventArgs e) => _backingDataModel?.CancelButtonClicked();
}
