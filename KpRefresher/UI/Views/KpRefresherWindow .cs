﻿using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Controls;
using KpRefresher.Services;
using Microsoft.Xna.Framework;
using System;
using System.Threading.Tasks;
using static Blish_HUD.ContentService;

namespace KpRefresher.UI.Views
{
    public class KpRefresherWindow : StandardWindow
    {
        private ModuleSettings _moduleSettings { get; set; }
        private BusinessService _businessService { get; set; }

        private LoadingSpinner _loadingSpinner { get; set; }
        private Panel _notificationsContainer { get; set; }
        private Label _notificationLabel { get; set; }
        private Checkbox _showAutoRetryNotificationCheckbox { get; set; }
        private Checkbox _refreshOnKillOnlyBossCheckbox { get; set; }

        public KpRefresherWindow(AsyncTexture2D background, Rectangle windowRegion, Rectangle contentRegion,
            AsyncTexture2D cornerIconTexture, ModuleSettings moduleSettings, BusinessService businessService) : base(background, windowRegion, contentRegion)
        {
            Parent = GameService.Graphics.SpriteScreen;
            Title = "Kp Refresher";
            Emblem = cornerIconTexture;
            Location = new Point(300, 300);
            SavesPosition = true;

            _moduleSettings = moduleSettings;
            _businessService = businessService;

            //Panel testPanel = new()
            //{
            //    Parent = this,
            //    BackgroundColor = Color.Red,
            //    WidthSizingMode = SizingMode.Fill,
            //    HeightSizingMode = SizingMode.Fill,
            //    ZIndex = 100
            //};
        }

        public void BuildUi()
        {
            FlowPanel mainContainer = new()
            {
                Parent = this,
                WidthSizingMode = SizingMode.Fill,
                HeightSizingMode = SizingMode.Fill,
                ControlPadding = new(3, 3)
            };

            #region Config
            FlowPanel configContainer = new()
            {
                Parent = mainContainer,
                WidthSizingMode = SizingMode.Fill,
                Height = 200,
                Title = "Configuration",
                ShowBorder = true,
                CanCollapse = true,
                OuterControlPadding = new(5),
                ControlPadding = new(3, 3),
                FlowDirection = ControlFlowDirection.TopToBottom
            };

            #region KpId
            var kpIdContainer = new FlowPanel()
            {
                Parent = configContainer,
                WidthSizingMode = SizingMode.Fill,
                HeightSizingMode = SizingMode.AutoSize,
                ControlPadding = new(3, 3),
                FlowDirection = ControlFlowDirection.SingleLeftToRight,
            };

            Label kpIdLabel = new()
            {
                Parent = kpIdContainer,
                AutoSizeWidth = true,
                Height = 25,
                Text = "Killproof.me Id : ",
                BasicTooltipText = "If empty, refresh will be made with the GW2 account name (e.g. Quaggan.1234)"
            };

            TextBox kpIdTextBox = new()
            {
                Parent = kpIdContainer,
                Width = 75,
                Text = _moduleSettings.KpMeId.Value,
            };
            kpIdTextBox.EnterPressed += async (s, e) =>
            {
                _moduleSettings.KpMeId.Value = kpIdTextBox.Text;
                await _businessService.UpdateLastRefresh();
            };
            #endregion KpId

            #region Auto retry
            var autoRetryContainer = new FlowPanel()
            {
                Parent = configContainer,
                WidthSizingMode = SizingMode.Fill,
                HeightSizingMode = SizingMode.AutoSize,
                ControlPadding = new(20, 3),
                FlowDirection = ControlFlowDirection.SingleLeftToRight
            };

            #region autoRetryEnable
            var autoRetryEnableContainer = new FlowPanel()
            {
                Parent = autoRetryContainer,
                WidthSizingMode = SizingMode.AutoSize,
                HeightSizingMode = SizingMode.AutoSize,
                ControlPadding = new(3, 3),
                FlowDirection = ControlFlowDirection.SingleLeftToRight
            };

            Label autoRetryEnableLabel = new()
            {
                Parent = autoRetryEnableContainer,
                AutoSizeWidth = true,
                Height = 25,
                Text = "Enable auto-retry : ",
                BasicTooltipText = "Schedule automatically a new try when killproof.me was not available for a refresh",
            };

            Checkbox autoRetryEnableCheckbox = new()
            {
                Parent = autoRetryEnableContainer,
                Checked = _moduleSettings.EnableAutoRetry.Value
            };
            autoRetryEnableCheckbox.CheckedChanged += (s, e) =>
            {
                _moduleSettings.EnableAutoRetry.Value = autoRetryEnableCheckbox.Checked;
                _showAutoRetryNotificationCheckbox.Enabled= autoRetryEnableCheckbox.Checked;
            };
            #endregion autoRetryEnable

            #region autoRetryNotification
            FlowPanel autoRetryNotificationContainer = new()
            {
                Parent = autoRetryContainer,
                WidthSizingMode = SizingMode.AutoSize,
                HeightSizingMode = SizingMode.AutoSize,
                ControlPadding = new(3, 3),
                FlowDirection = ControlFlowDirection.SingleLeftToRight
            };

            Label showAutoRetryNotificationLabel = new()
            {
                Parent = autoRetryNotificationContainer,
                AutoSizeWidth = true,
                Height = 25,
                Text = "Show auto-retry notifications : ",
                BasicTooltipText = "Display notification when retry is scheduled",
            };

            _showAutoRetryNotificationCheckbox = new Checkbox()
            {
                Parent = autoRetryNotificationContainer,
                Checked = _moduleSettings.ShowScheduleNotification.Value,
                Enabled = _moduleSettings.EnableAutoRetry.Value
            };
            _showAutoRetryNotificationCheckbox.CheckedChanged += (s, e) =>
            {
                _moduleSettings.ShowScheduleNotification.Value = _showAutoRetryNotificationCheckbox.Checked;
            };
            #endregion autoRetryNotification
            #endregion Auto retry

            #region Refresh only if boss clear
            FlowPanel refreshOnKillContainer = new()
            {
                Parent = configContainer,
                WidthSizingMode = SizingMode.Fill,
                HeightSizingMode = SizingMode.AutoSize,
                ControlPadding = new(20, 3),
                FlowDirection = ControlFlowDirection.SingleLeftToRight
            };

            #region refreshOnKillEnable
            FlowPanel refreshOnKillEnableContainer = new()
            {
                Parent = refreshOnKillContainer,
                WidthSizingMode = SizingMode.AutoSize,
                HeightSizingMode = SizingMode.AutoSize,
                ControlPadding = new(3, 3),
                FlowDirection = ControlFlowDirection.SingleLeftToRight
            };

            Label refreshOnKillEnableLabel = new()
            {
                Parent = refreshOnKillEnableContainer,
                AutoSizeWidth = true,
                Height = 25,
                Text = "Condition refresh to clear : ",
                BasicTooltipText = "Only allow refresh if a clear was made and is visible by GW2 API",
            };

            Checkbox refreshOnKillEnableCheckbox = new()
            {
                Parent = refreshOnKillEnableContainer,
                Checked = _moduleSettings.EnableRefreshOnKill.Value
            };
            refreshOnKillEnableCheckbox.CheckedChanged += (s, e) =>
            {
                _moduleSettings.EnableRefreshOnKill.Value = refreshOnKillEnableCheckbox.Checked;
                _refreshOnKillOnlyBossCheckbox.Enabled = refreshOnKillEnableCheckbox.Checked;
            };
            #endregion refreshOnKillEnable

            #region refreshOnKillOnlyBossNotification
            FlowPanel refreshOnKillOnlyBossContainer = new()
            {
                Parent = refreshOnKillContainer,
                WidthSizingMode = SizingMode.AutoSize,
                HeightSizingMode = SizingMode.AutoSize,
                ControlPadding = new(3, 3),
                FlowDirection = ControlFlowDirection.SingleLeftToRight
            };

            Label refreshOnKillOnlyBossLabel = new()
            {
                Parent = refreshOnKillOnlyBossContainer,
                AutoSizeWidth = true,
                Height = 25,
                Text = "Refresh on final boss kill : ",
                BasicTooltipText = "Only refresh if a final raid wing boss was cleared (e.g. Sabetha)",
            };

            _refreshOnKillOnlyBossCheckbox = new Checkbox()
            {
                Parent = refreshOnKillOnlyBossContainer,
                Checked = _moduleSettings.RefreshOnKillOnlyBoss.Value,
                Enabled = _moduleSettings.EnableRefreshOnKill.Value
            };
            _refreshOnKillOnlyBossCheckbox.CheckedChanged += (s, e) =>
            {
                _moduleSettings.RefreshOnKillOnlyBoss.Value = _refreshOnKillOnlyBossCheckbox.Checked;
            };
            #endregion refreshOnKillOnlyBoss
            #endregion Refresh only if boss clear

            #endregion Config

            #region Actions
            FlowPanel actionContainer = new()
            {
                Parent = mainContainer,
                WidthSizingMode = SizingMode.Fill,
                HeightSizingMode = SizingMode.AutoSize,
                Title = "Actions",
                ShowBorder = true,
                CanCollapse = true,
                OuterControlPadding = new(5),
                ControlPadding = new(3, 3),
                FlowDirection = ControlFlowDirection.SingleTopToBottom
            };

            #region Line1
            FlowPanel actionLine1Container = new()
            {
                Parent = actionContainer,
                WidthSizingMode = SizingMode.Fill,
                HeightSizingMode = SizingMode.AutoSize,
                ControlPadding = new(3, 3),
                FlowDirection = ControlFlowDirection.SingleLeftToRight
            };

            #region RefreshRaidClears
            StandardButton refreshRaidClears = new()
            {
                Parent = actionLine1Container,
                Size = new Point(150, 30),
                Text = "Refresh killproof.me",
                BasicTooltipText = "Attempts to refresh killproof.me\nIf auto-retry is enable, a new refresh will be scheduled in case of failure",
            };
            refreshRaidClears.Click += async (s, e) =>
            {
                await RefreshRaidClears();
            };
            #endregion RefreshRaidClears

            #region StopRetry
            StandardButton stopRetry = new()
            {
                Parent = actionLine1Container,
                Size = new Point(130, 30),
                Text = "Stop retry",
                BasicTooltipText = "Resets any scheduled refresh",
            };
            stopRetry.Click += (s, e) =>
            {
                StopRetry();
            };
            #endregion StopRetry

            #region Spinner
            _loadingSpinner = new LoadingSpinner()
            {
                Parent = actionLine1Container,
                Size = new Point(29, 29),
                Visible = false,
            };
            _loadingSpinner.MouseEntered += (s, e) =>
            {
                _loadingSpinner.BasicTooltipText = $"Next retry in {Math.Round(_businessService.GetNextScheduledTimer() / 60 / 1000)} minutes.";
            };
            #endregion Spinner
            #endregion Line1

            #region Line2
            FlowPanel actionLine2Container = new()
            {
                Parent = actionContainer,
                WidthSizingMode = SizingMode.Fill,
                HeightSizingMode = SizingMode.AutoSize,
                ControlPadding = new(3, 3),
                FlowDirection = ControlFlowDirection.SingleLeftToRight
            };

            #region DisplayRaidDifference
            StandardButton displayRaidDifference = new()
            {
                Parent = actionLine2Container,
                Size = new Point(150, 30),
                Text = "Show new clears",
                BasicTooltipText = "Displays new kills made since KpRefresher start or last successful killproof.me refresh",
            };
            displayRaidDifference.Click += async (s, e) =>
            {
                await DisplayRaidDifference();
            };
            #endregion DisplayRaidDifference

            #region DisplayCurrentKp
            StandardButton displayCurrentKp = new()
            {
                Parent = actionLine2Container,
                Size = new Point(150, 30),
                Text = "Show current KP",
                BasicTooltipText = "Scan your bank, shared slots and characters and displays current KP according GW2 API.\nEvery kp in the list is able to be scanned by killproof.me, if not already scanned. You can use this feature to check if a newly opened chest is already visible for killproof.me.",
            };
            displayCurrentKp.Click += async (s, e) =>
            {
                await DisplayCurrentKp();
            };
            #endregion DisplayCurrentKp

            #region ClearNotifications
            StandardButton clearNotifications = new()
            {
                Parent = actionLine2Container,
                Size = new Point(150, 30),
                Text = "Clear notifications"
            };
            clearNotifications.Click += (s, e) =>
            {
                ClearNotifications();
            };
            #endregion ClearNotifications
            #endregion Line2
            #endregion Actions

            #region Notifications
            _notificationsContainer = new()
            {
                Parent = mainContainer,
                HeightSizingMode = SizingMode.Fill,
                WidthSizingMode = SizingMode.Fill,
            };

            _notificationLabel = new Label()
            {
                Parent = _notificationsContainer,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Font = GameService.Content.GetFont(FontFace.Menomonia, FontSize.Size24, FontStyle.Regular),
                WrapText = true
            };
            #endregion Notifications
        }

        private async Task RefreshRaidClears()
        {
            //TODO: maybe disable Refresh btn if we can find a way to auto reactivate it from Service ?

            _loadingSpinner.Visible = true;

            await _businessService.RefreshKillproofMe();

            //Keeps the spinner visible in refresh in auto-retry
            _loadingSpinner.Visible = _businessService.RefreshScheduled;
        }

        private async Task DisplayRaidDifference()
        {
            ShowInsideNotification("Loading ...", true);

            var data = await _businessService.GetDelta();
            ShowInsideNotification(data, true);
        }

        private void StopRetry()
        {
            if (_businessService.RefreshScheduled)
            {
                _businessService.CancelSchedule();
                ShowInsideNotification("Auto-retry disabled !");
            }
            else
            {
                ShowInsideNotification("No scheduled refresh");
            }

            _loadingSpinner.Visible = false;
        }

        private void ShowInsideNotification(string message, bool persistMessage = false)
        {
            _notificationLabel.Text = message;
            _notificationLabel.Visible = true;
            _notificationLabel.Width = _notificationsContainer.Width;
            _notificationLabel.Height = _notificationsContainer.Height;

            if (!persistMessage)
            {
                Task.Run(async delegate
                {
                    await Task.Delay(4000);

                    ClearNotifications();

                    return;
                });
            }
        }

        private async Task DisplayCurrentKp()
        {
            ShowInsideNotification("Loading ...", true);

            var data = await _businessService.DisplayCurrentKp();
            ShowInsideNotification(data, true);
        }

        private void ClearNotifications()
        {
            _notificationLabel.Text = string.Empty;
            _notificationLabel.Visible = false;
        }
    }
}
