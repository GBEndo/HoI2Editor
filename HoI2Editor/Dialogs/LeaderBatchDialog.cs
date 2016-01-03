﻿using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HoI2Editor.Models;
using HoI2Editor.Utilities;

namespace HoI2Editor.Dialogs
{
    /// <summary>
    ///     指揮官一括編集ダイアログ
    /// </summary>
    public partial class LeaderBatchDialog : Form
    {
        #region 内部フィールド

        /// <summary>
        ///     一括編集のパラメータ
        /// </summary>
        private readonly LeaderBatchEditArgs _args;

        /// <summary>
        ///     開始IDが変更されたかどうか
        /// </summary>
        private bool _idChanged;

        #endregion

        #region 初期化

        /// <summary>
        ///     コンストラクタ
        /// </summary>
        /// <param name="args">一括編集のパラメータ</param>
        public LeaderBatchDialog(LeaderBatchEditArgs args)
        {
            InitializeComponent();

            _args = args;
        }

        #endregion

        #region フォーム

        /// <summary>
        ///     フォーム読み込み時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFormLoad(object sender, EventArgs e)
        {
            Graphics g = Graphics.FromHwnd(Handle);
            int margin = DeviceCaps.GetScaledWidth(2) + 1;

            // 対象国コンボボックス
            targetCountryComboBox.BeginUpdate();
            targetCountryComboBox.Items.Clear();
            int width = targetCountryComboBox.Width;
            foreach (string s in Countries.Tags
                .Select(country => Countries.Strings[(int) country])
                .Select(name => Config.ExistsKey(name)
                    ? $"{name} {Config.GetText(name)}"
                    : name))
            {
                targetCountryComboBox.Items.Add(s);
                width = Math.Max(width,
                    (int) g.MeasureString(s, targetCountryComboBox.Font).Width +
                    SystemInformation.VerticalScrollBarWidth +
                    margin);
            }
            targetCountryComboBox.DropDownWidth = width;
            targetCountryComboBox.EndUpdate();
            if (_args.TargetCountries.Count > 0)
            {
                targetCountryComboBox.SelectedIndex = Countries.Tags.ToList().IndexOf(_args.TargetCountries[0]);
            }
            targetCountryComboBox.SelectedIndexChanged += OnTargetCountryComboBoxSelectedIndexChanged;

            // 理想階級コンボボックス
            idealRankComboBox.BeginUpdate();
            idealRankComboBox.Items.Clear();
            width = idealRankComboBox.Width;
            foreach (string s in Leaders.RankNames.Where(name => !string.IsNullOrEmpty(name)))
            {
                idealRankComboBox.Items.Add(s);
                width = Math.Max(width, (int) g.MeasureString(s, idealRankComboBox.Font).Width + margin);
            }
            idealRankComboBox.DropDownWidth = width;
            idealRankComboBox.EndUpdate();
            if (idealRankComboBox.Items.Count > 0)
            {
                idealRankComboBox.SelectedIndex = 0;
            }
            idealRankComboBox.SelectedIndexChanged += OnIdealRankComboBoxSelectedIndexChanged;

            // 引退年
            if ((Game.Type != GameType.DarkestHour) || (Game.Version < 103))
            {
                retirementYearCheckBox.Enabled = false;
                retirementYearNumericUpDown.Enabled = false;
                retirementYearNumericUpDown.ResetText();
            }

            // コピー先コンボボックス
            copyCountryComboBox.BeginUpdate();
            copyCountryComboBox.Items.Clear();
            width = copyCountryComboBox.Width;
            foreach (string s in Countries.Tags
                .Select(country => Countries.Strings[(int) country])
                .Select(name => Config.ExistsKey(name)
                    ? $"{name} {Config.GetText(name)}"
                    : name))
            {
                copyCountryComboBox.Items.Add(s);
                width = Math.Max(width,
                    (int) g.MeasureString(s, targetCountryComboBox.Font).Width +
                    SystemInformation.VerticalScrollBarWidth +
                    margin);
            }
            copyCountryComboBox.DropDownWidth = width;
            copyCountryComboBox.EndUpdate();
            if (_args.TargetCountries.Count > 0)
            {
                copyCountryComboBox.SelectedIndex = Countries.Tags.ToList().IndexOf(_args.TargetCountries[0]);
            }
            copyCountryComboBox.SelectedIndexChanged += OnCopyCountryComboBoxSelectedIndexChanged;

            // 開始ID
            if (_args.TargetCountries.Count > 0)
            {
                idNumericUpDown.Value = Leaders.GetNewId(_args.TargetCountries[0]);
            }
            idNumericUpDown.ValueChanged += OnIdNumericUpDownValueChanged;
        }

        /// <summary>
        ///     OKキー押下時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnOkButtonClick(object sender, EventArgs e)
        {
            if (allRadioButton.Checked)
            {
                _args.Mode = BatchMode.All;
            }
            else if (selectedRadioButton.Checked)
            {
                _args.Mode = BatchMode.Selected;
            }
            else
            {
                _args.Mode = BatchMode.Specified;
                _args.TargetCountries.Clear();
                _args.TargetCountries.Add(Countries.Tags[targetCountryComboBox.SelectedIndex]);
            }

            _args.Army = armyCheckBox.Checked;
            _args.Navy = navyCheckBox.Checked;
            _args.Airforce = airforceCheckBox.Checked;

            _args.Items[(int) LeaderBatchItemId.IdealRank] = idealRankCheckBox.Checked;
            _args.Items[(int) LeaderBatchItemId.Skill] = skillCheckBox.Checked;
            _args.Items[(int) LeaderBatchItemId.MaxSkill] = maxSkillCheckBox.Checked;
            _args.Items[(int) LeaderBatchItemId.Experience] = experienceCheckBox.Checked;
            _args.Items[(int) LeaderBatchItemId.Loyalty] = loyaltyCheckBox.Checked;
            _args.Items[(int) LeaderBatchItemId.StartYear] = startYearCheckBox.Checked;
            _args.Items[(int) LeaderBatchItemId.EndYear] = endYearCheckBox.Checked;
            _args.Items[(int) LeaderBatchItemId.RetirementYear] = retirementYearCheckBox.Checked;
            _args.Items[(int) LeaderBatchItemId.Rank3Year] = rankYearCheckBox1.Checked;
            _args.Items[(int) LeaderBatchItemId.Rank2Year] = rankYearCheckBox2.Checked;
            _args.Items[(int) LeaderBatchItemId.Rank1Year] = rankYearCheckBox3.Checked;
            _args.Items[(int) LeaderBatchItemId.Rank0Year] = rankYearCheckBox4.Checked;

            _args.IdealRank = (LeaderRank) (idealRankComboBox.SelectedIndex + 1);
            _args.Skill = (int) skillNumericUpDown.Value;
            _args.MaxSkill = (int) maxSkillNumericUpDown.Value;
            _args.Experience = (int) experienceNumericUpDown.Value;
            _args.Loyalty = (int) loyaltyNumericUpDown.Value;
            _args.StartYear = (int) startYearNumericUpDown.Value;
            _args.EndYear = (int) endYearNumericUpDown.Value;
            _args.RetirementYear = (int) retirementYearNumericUpDown.Value;
            _args.RankYear[0] = (int) rankYearNumericUpDown1.Value;
            _args.RankYear[1] = (int) rankYearNumericUpDown2.Value;
            _args.RankYear[2] = (int) rankYearNumericUpDown3.Value;
            _args.RankYear[3] = (int) rankYearNumericUpDown4.Value;

            _args.CopyCountry = copyToCheckBox.Checked
                ? Countries.Tags[copyCountryComboBox.SelectedIndex]
                : Country.None;
            _args.Id = (int) idNumericUpDown.Value;
        }

        #endregion

        #region 編集項目

        /// <summary>
        ///     対象国コンボボックスの選択項目変更時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTargetCountryComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (targetCountryComboBox.SelectedIndex < 0)
            {
                return;
            }
            specifiedRadioButton.Checked = true;
        }

        /// <summary>
        ///     理想階級コンボボックスの選択項目変更時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnIdealRankComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            idealRankCheckBox.Checked = true;
        }

        /// <summary>
        ///     スキル数値アップダウンの値変更時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSkillNumericUpDownValueChanged(object sender, EventArgs e)
        {
            skillCheckBox.Checked = true;
        }

        /// <summary>
        ///     最大スキル数値アップダウンの値変更時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMaxSkillNumericUpDownValueChanged(object sender, EventArgs e)
        {
            maxSkillCheckBox.Checked = true;
        }

        /// <summary>
        ///     経験値数値アップダウンの値変更時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnExperienceNumericUpDownValueChanged(object sender, EventArgs e)
        {
            experienceCheckBox.Checked = true;
        }

        /// <summary>
        ///     忠誠度数値アップダウンの値変更時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoyaltyNumericUpDownValueChanged(object sender, EventArgs e)
        {
            loyaltyCheckBox.Checked = true;
        }

        /// <summary>
        ///     開始年数値アップダウンの値変更時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStartYearNumericUpDownValueChanged(object sender, EventArgs e)
        {
            startYearCheckBox.Checked = true;
        }

        /// <summary>
        ///     終了年数値アップダウンの値変更時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEndYearNumericUpDownValueChanged(object sender, EventArgs e)
        {
            endYearCheckBox.Checked = true;
        }

        /// <summary>
        ///     引退年数値アップダウンの値変更時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRetirementYearNumericUpDownValueChanged(object sender, EventArgs e)
        {
            retirementYearCheckBox.Checked = true;
        }

        /// <summary>
        ///     少将任官年数値アップダウンの値変更時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRankYearNumericUpDown1ValueChanged(object sender, EventArgs e)
        {
            rankYearCheckBox1.Checked = true;
        }

        /// <summary>
        ///     中将任官年数値アップダウンの値変更時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRankYearNumericUpDown2ValueChanged(object sender, EventArgs e)
        {
            rankYearCheckBox2.Checked = true;
        }

        /// <summary>
        ///     大将任官年数値アップダウンの値変更時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRankYearNumericUpDown3ValueChanged(object sender, EventArgs e)
        {
            rankYearCheckBox3.Checked = true;
        }

        /// <summary>
        ///     元帥任官年数値アップダウンの値変更時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRankYearNumericUpDown4ValueChanged(object sender, EventArgs e)
        {
            rankYearCheckBox4.Checked = true;
        }

        /// <summary>
        ///     コピー先コンボボックスの選択項目変更時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCopyCountryComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (copyCountryComboBox.SelectedIndex < 0)
            {
                return;
            }
            copyToCheckBox.Checked = true;

            // 開始ID数値アップダウンの数値が変更されていなければ変更する
            if (!_idChanged)
            {
                idNumericUpDown.ValueChanged -= OnIdNumericUpDownValueChanged;
                idNumericUpDown.Value = Leaders.GetNewId(Countries.Tags[copyCountryComboBox.SelectedIndex]);
                idNumericUpDown.ValueChanged += OnIdNumericUpDownValueChanged;
            }
        }

        /// <summary>
        ///     ID数値アップダウンの値変更時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnIdNumericUpDownValueChanged(object sender, EventArgs e)
        {
            _idChanged = true;
        }

        #endregion
    }
}