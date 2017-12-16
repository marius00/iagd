using EvilsoftCommons.UI;
using IAGrim.Backup;
using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.Utilities;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using IAGrim.Parsers.Arz;

namespace IAGrim.UI.Popups {
    public partial class OnlineBackupLogin : Form {
        private static readonly ILog logger = LogManager.GetLogger(typeof(OnlineBackupLogin));
        TextboxPlaceholder tbPlaceholder;
        private BackgroundWorker bw;
        private readonly IPlayerItemDao _playerItemDao;

        public OnlineBackupLogin(IPlayerItemDao playerItemDao) {
            InitializeComponent();
            tbPlaceholder = new TextboxPlaceholder(tbEmail, "your@email.com");
            this._playerItemDao = playerItemDao;

            this.FormClosing += OnlineBackupLogin_FormClosing;
        }

        private void OnlineBackupLogin_FormClosing(object sender, FormClosingEventArgs e) {
            bw?.CancelAsync();
        }

        private void buttonLogin_Click(object sender, EventArgs e) {
            // This is currently a 'cancel' button
            if (!tbEmail.Visible) {
                Properties.Settings.Default.BackupLogin = string.Empty;
                Properties.Settings.Default.Save();
                SetState(LoginState.NEED_TO_LOGIN);
            }

            // This is a login button
            else {
                SetState(LoginState.LOGGING_IN);

                bw?.CancelAsync();
                bw = new BackgroundWorker { WorkerSupportsCancellation = true, WorkerReportsProgress = true };
                bw.DoWork += bw_Login;
                bw.ProgressChanged += Bw_ProgressChanged;
                bw.WorkerSupportsCancellation = true;
                bw.RunWorkerAsync();
            }
        }

        private void Bw_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            if (this.InvokeRequired) {
                this.Invoke((MethodInvoker)delegate {
                    Bw_ProgressChanged(sender, e);
                });
            } else {
                if (e.ProgressPercentage == 1) {
                    this.labelVerify.Visible = true;
                    SetState(LoginState.JUST_SENT_EMAIL);

                    StartWaitForVerifyBackgroundWorker();
                }
                else if (e.ProgressPercentage == 3) {
                    logger.Info("Online backup login token confirmed");
                    Properties.Settings.Default.OnlineBackupVerified = true;
                    Properties.Settings.Default.Save();
                    this.Close();
                }
                else {
                    MessageBox.Show($"Error logging into online backup\n{e.UserState.ToString()}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    SetState(LoginState.NEED_TO_LOGIN);
                }
            }
        }

        // TODO: Create a background worker which every 10 seconds checks for online verification status
        private void bw_verify(object sender, DoWorkEventArgs e) {
            var bw = sender as BackgroundWorker;
            var sync = new ItemSynchronizer(_playerItemDao, string.Empty, GlobalSettings.RemoteBackupServer, null);

            Stopwatch sw = new Stopwatch();
            sw.Start();
            
            while (!bw.CancellationPending) {
                if (sw.ElapsedMilliseconds > 10 * 1000) {
                    if (sync.Verify(Properties.Settings.Default.OnlineBackupToken)) {
                        bw.ReportProgress(3);
                        return;
                    }
                    sw.Restart();
                }
                Thread.Sleep(10);
            }
        }

        private void StartWaitForVerifyBackgroundWorker() {
            bw?.CancelAsync();
            bw = new BackgroundWorker { WorkerSupportsCancellation = true, WorkerReportsProgress = true };
            bw.DoWork += bw_verify;
            bw.ProgressChanged += Bw_ProgressChanged;
            bw.WorkerSupportsCancellation = true;
            bw.RunWorkerAsync();
            logger.Info("Querying for verification token..");
        }

        private void bw_Login(object sender, DoWorkEventArgs e) {
            var bw = sender as BackgroundWorker;
            string error;

            if (Thread.CurrentThread.Name == null)
                Thread.CurrentThread.Name = "BackupLogin";

            Properties.Settings.Default.OnlineBackupEmail = tbEmail.Text;
            Properties.Settings.Default.Save();

            var token = new ItemSynchronizer(_playerItemDao, string.Empty, GlobalSettings.RemoteBackupServer, null)
                .Login(tbEmail.Text, out error);

            if (!string.IsNullOrEmpty(token)) {
                Properties.Settings.Default.OnlineBackupToken = token;
                Properties.Settings.Default.Save();
                bw.ReportProgress(1);
            } else {
                bw.ReportProgress(2, error);
            }
        }

        enum LoginState {
            WAITING_FOR_CONFIRM,
            NEED_TO_LOGIN,
            JUST_SENT_EMAIL,
            LOGGING_IN
        };
        private void SetState(LoginState state) {
            switch (state) {
                case LoginState.WAITING_FOR_CONFIRM:
                    buttonLogin.Text = "Re-Send";
                    labelVerify.Visible = true;
                    labelCheckSpam.Visible = true;
                    tbEmail.Visible = false;
                    buttonLogin.Enabled = true;
                    buttonLogin.Visible = true;
                    progressBar1.Visible = false;
                    break;

                case LoginState.NEED_TO_LOGIN:
                    buttonLogin.Text = "Login";
                    labelVerify.Visible = false;
                    labelCheckSpam.Visible = false;
                    tbEmail.Visible = true;
                    tbEmail.Enabled = true;
                    buttonLogin.Enabled = true;
                    buttonLogin.Visible = true;
                    progressBar1.Visible = false;
                    break;

                case LoginState.JUST_SENT_EMAIL:
                    tbEmail.Visible = false;
                    buttonLogin.Visible = false;
                    labelVerify.Visible = true;
                    labelCheckSpam.Visible = true;
                    progressBar1.Visible = false;
                    break;

                case LoginState.LOGGING_IN:
                    tbEmail.Enabled = false;
                    buttonLogin.Enabled = false;
                    tbEmail.Visible = true;
                    buttonLogin.Visible = true;
                    progressBar1.Visible = true;
                    break;
            }
        }

        private void OnlineBackupLogin_Load(object sender, EventArgs e) {
            if (!string.IsNullOrEmpty(Properties.Settings.Default.BackupLogin)) {
                SetState(LoginState.WAITING_FOR_CONFIRM);
                StartWaitForVerifyBackgroundWorker();
            } else {
                SetState(LoginState.NEED_TO_LOGIN);
            }

            tbEmail.KeyDown += TbEmail_KeyDown;

            LocalizationLoader.ApplyLanguage(Controls, GlobalSettings.Language);
        }

        private void TbEmail_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter)
                buttonLogin_Click(sender, e);
        }
    }
}
