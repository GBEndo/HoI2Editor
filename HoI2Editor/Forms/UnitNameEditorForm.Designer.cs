﻿namespace HoI2Editor.Forms
{
    partial class UnitNameEditorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UnitNameEditorForm));
            this.countryListBox = new System.Windows.Forms.ListBox();
            this.closeButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.reloadButton = new System.Windows.Forms.Button();
            this.typeListBox = new System.Windows.Forms.ListBox();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.optionGroupBox = new System.Windows.Forms.GroupBox();
            this.escortFighterCheckBox = new System.Windows.Forms.CheckBox();
            this.allUnitTypeCheckBox = new System.Windows.Forms.CheckBox();
            this.allCountryCheckBox = new System.Windows.Forms.CheckBox();
            this.regexcheckBox = new System.Windows.Forms.CheckBox();
            this.replaceButton = new System.Windows.Forms.Button();
            this.findComboBox = new System.Windows.Forms.ComboBox();
            this.replaceComboBox = new System.Windows.Forms.ComboBox();
            this.startNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.endNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.interpolateButton = new System.Windows.Forms.Button();
            this.interpolateComboBox = new System.Windows.Forms.ComboBox();
            this.addButton = new System.Windows.Forms.Button();
            this.replaceGroupBox = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.findLabel = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.suffixLabel = new System.Windows.Forms.Label();
            this.cutButton = new System.Windows.Forms.Button();
            this.pasteButton = new System.Windows.Forms.Button();
            this.undoButton = new System.Windows.Forms.Button();
            this.copyButton = new System.Windows.Forms.Button();
            this.optionGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.startNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.endNumericUpDown)).BeginInit();
            this.replaceGroupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // countryListBox
            // 
            this.countryListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.countryListBox.FormattingEnabled = true;
            resources.ApplyResources(this.countryListBox, "countryListBox");
            this.countryListBox.Name = "countryListBox";
            this.countryListBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.OnCountryListBoxDrawItem);
            this.countryListBox.SelectedIndexChanged += new System.EventHandler(this.OnCountryListBoxSelectedIndexChanged);
            // 
            // closeButton
            // 
            resources.ApplyResources(this.closeButton, "closeButton");
            this.closeButton.Name = "closeButton";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.OnCloseButtonClick);
            // 
            // saveButton
            // 
            resources.ApplyResources(this.saveButton, "saveButton");
            this.saveButton.Name = "saveButton";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.OnSaveButtonClick);
            // 
            // reloadButton
            // 
            resources.ApplyResources(this.reloadButton, "reloadButton");
            this.reloadButton.Name = "reloadButton";
            this.reloadButton.UseVisualStyleBackColor = true;
            this.reloadButton.Click += new System.EventHandler(this.OnReloadButtonClick);
            // 
            // typeListBox
            // 
            this.typeListBox.FormattingEnabled = true;
            resources.ApplyResources(this.typeListBox, "typeListBox");
            this.typeListBox.Name = "typeListBox";
            this.typeListBox.SelectedIndexChanged += new System.EventHandler(this.OnTypeListBoxSelectedIndexChanged);
            // 
            // nameTextBox
            // 
            resources.ApplyResources(this.nameTextBox, "nameTextBox");
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Validated += new System.EventHandler(this.OnNameTextBoxValidated);
            // 
            // optionGroupBox
            // 
            this.optionGroupBox.Controls.Add(this.escortFighterCheckBox);
            this.optionGroupBox.Controls.Add(this.allUnitTypeCheckBox);
            this.optionGroupBox.Controls.Add(this.allCountryCheckBox);
            resources.ApplyResources(this.optionGroupBox, "optionGroupBox");
            this.optionGroupBox.Name = "optionGroupBox";
            this.optionGroupBox.TabStop = false;
            // 
            // escortFighterCheckBox
            // 
            resources.ApplyResources(this.escortFighterCheckBox, "escortFighterCheckBox");
            this.escortFighterCheckBox.Checked = true;
            this.escortFighterCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.escortFighterCheckBox.Name = "escortFighterCheckBox";
            this.escortFighterCheckBox.UseVisualStyleBackColor = true;
            // 
            // allUnitTypeCheckBox
            // 
            resources.ApplyResources(this.allUnitTypeCheckBox, "allUnitTypeCheckBox");
            this.allUnitTypeCheckBox.Name = "allUnitTypeCheckBox";
            this.allUnitTypeCheckBox.UseVisualStyleBackColor = true;
            // 
            // allCountryCheckBox
            // 
            resources.ApplyResources(this.allCountryCheckBox, "allCountryCheckBox");
            this.allCountryCheckBox.Name = "allCountryCheckBox";
            this.allCountryCheckBox.UseVisualStyleBackColor = true;
            // 
            // regexcheckBox
            // 
            resources.ApplyResources(this.regexcheckBox, "regexcheckBox");
            this.regexcheckBox.Checked = true;
            this.regexcheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.regexcheckBox.Name = "regexcheckBox";
            this.regexcheckBox.UseVisualStyleBackColor = true;
            // 
            // replaceButton
            // 
            resources.ApplyResources(this.replaceButton, "replaceButton");
            this.replaceButton.Name = "replaceButton";
            this.replaceButton.UseVisualStyleBackColor = true;
            this.replaceButton.Click += new System.EventHandler(this.OnReplaceButtonClick);
            // 
            // findComboBox
            // 
            this.findComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.findComboBox, "findComboBox");
            this.findComboBox.Name = "findComboBox";
            // 
            // replaceComboBox
            // 
            this.replaceComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.replaceComboBox, "replaceComboBox");
            this.replaceComboBox.Name = "replaceComboBox";
            // 
            // startNumericUpDown
            // 
            resources.ApplyResources(this.startNumericUpDown, "startNumericUpDown");
            this.startNumericUpDown.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.startNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.startNumericUpDown.Name = "startNumericUpDown";
            this.startNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // endNumericUpDown
            // 
            resources.ApplyResources(this.endNumericUpDown, "endNumericUpDown");
            this.endNumericUpDown.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.endNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.endNumericUpDown.Name = "endNumericUpDown";
            this.endNumericUpDown.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // interpolateButton
            // 
            resources.ApplyResources(this.interpolateButton, "interpolateButton");
            this.interpolateButton.Name = "interpolateButton";
            this.interpolateButton.UseVisualStyleBackColor = true;
            // 
            // interpolateComboBox
            // 
            this.interpolateComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.interpolateComboBox, "interpolateComboBox");
            this.interpolateComboBox.Name = "interpolateComboBox";
            // 
            // addButton
            // 
            resources.ApplyResources(this.addButton, "addButton");
            this.addButton.Name = "addButton";
            this.addButton.UseVisualStyleBackColor = true;
            // 
            // replaceGroupBox
            // 
            this.replaceGroupBox.Controls.Add(this.label1);
            this.replaceGroupBox.Controls.Add(this.findLabel);
            this.replaceGroupBox.Controls.Add(this.regexcheckBox);
            this.replaceGroupBox.Controls.Add(this.replaceButton);
            this.replaceGroupBox.Controls.Add(this.findComboBox);
            this.replaceGroupBox.Controls.Add(this.replaceComboBox);
            resources.ApplyResources(this.replaceGroupBox, "replaceGroupBox");
            this.replaceGroupBox.Name = "replaceGroupBox";
            this.replaceGroupBox.TabStop = false;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // findLabel
            // 
            resources.ApplyResources(this.findLabel, "findLabel");
            this.findLabel.Name = "findLabel";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.suffixLabel);
            this.groupBox1.Controls.Add(this.addButton);
            this.groupBox1.Controls.Add(this.interpolateComboBox);
            this.groupBox1.Controls.Add(this.startNumericUpDown);
            this.groupBox1.Controls.Add(this.interpolateButton);
            this.groupBox1.Controls.Add(this.endNumericUpDown);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // suffixLabel
            // 
            resources.ApplyResources(this.suffixLabel, "suffixLabel");
            this.suffixLabel.Name = "suffixLabel";
            // 
            // cutButton
            // 
            resources.ApplyResources(this.cutButton, "cutButton");
            this.cutButton.Name = "cutButton";
            this.cutButton.UseVisualStyleBackColor = true;
            this.cutButton.Click += new System.EventHandler(this.OnCutButtonClick);
            // 
            // pasteButton
            // 
            resources.ApplyResources(this.pasteButton, "pasteButton");
            this.pasteButton.Name = "pasteButton";
            this.pasteButton.UseVisualStyleBackColor = true;
            this.pasteButton.Click += new System.EventHandler(this.OnPasteButtonClick);
            // 
            // undoButton
            // 
            resources.ApplyResources(this.undoButton, "undoButton");
            this.undoButton.Name = "undoButton";
            this.undoButton.UseVisualStyleBackColor = true;
            this.undoButton.Click += new System.EventHandler(this.OnUndoButtonClick);
            // 
            // copyButton
            // 
            resources.ApplyResources(this.copyButton, "copyButton");
            this.copyButton.Name = "copyButton";
            this.copyButton.UseVisualStyleBackColor = true;
            this.copyButton.Click += new System.EventHandler(this.OnCopyButtonClick);
            // 
            // UnitNameEditorForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.copyButton);
            this.Controls.Add(this.undoButton);
            this.Controls.Add(this.pasteButton);
            this.Controls.Add(this.cutButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.replaceGroupBox);
            this.Controls.Add(this.optionGroupBox);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.typeListBox);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.reloadButton);
            this.Controls.Add(this.countryListBox);
            this.Name = "UnitNameEditorForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnMinisterEditorFormClosing);
            this.Load += new System.EventHandler(this.OnUnitNameEditorFormLoad);
            this.optionGroupBox.ResumeLayout(false);
            this.optionGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.startNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.endNumericUpDown)).EndInit();
            this.replaceGroupBox.ResumeLayout(false);
            this.replaceGroupBox.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox countryListBox;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button reloadButton;
        private System.Windows.Forms.ListBox typeListBox;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.GroupBox optionGroupBox;
        private System.Windows.Forms.CheckBox escortFighterCheckBox;
        private System.Windows.Forms.CheckBox allUnitTypeCheckBox;
        private System.Windows.Forms.CheckBox allCountryCheckBox;
        private System.Windows.Forms.CheckBox regexcheckBox;
        private System.Windows.Forms.Button replaceButton;
        private System.Windows.Forms.ComboBox findComboBox;
        private System.Windows.Forms.ComboBox replaceComboBox;
        private System.Windows.Forms.NumericUpDown startNumericUpDown;
        private System.Windows.Forms.NumericUpDown endNumericUpDown;
        private System.Windows.Forms.Button interpolateButton;
        private System.Windows.Forms.ComboBox interpolateComboBox;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.GroupBox replaceGroupBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label findLabel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label suffixLabel;
        private System.Windows.Forms.Button cutButton;
        private System.Windows.Forms.Button pasteButton;
        private System.Windows.Forms.Button undoButton;
        private System.Windows.Forms.Button copyButton;
    }
}