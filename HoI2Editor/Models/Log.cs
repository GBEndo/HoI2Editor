﻿using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using HoI2Editor.Parsers;
using HoI2Editor.Properties;
using HoI2Editor.Utilities;

namespace HoI2Editor.Models
{
    /// <summary>
    ///     ログの管理
    /// </summary>
    public static class Log
    {
        #region 公開プロパティ

        /// <summary>
        ///     ログ出力レベル
        /// </summary>
        public static int Level
        {
            get { return (int) Sw.Level; }
            set
            {
                if (Sw.Level == TraceLevel.Off)
                {
                    if (value > 0)
                    {
                        Init();
                    }
                }
                else
                {
                    if (value == 0)
                    {
                        Terminate();
                    }
                }
                if ((TraceLevel) value == Sw.Level)
                {
                    return;
                }
                Sw.Level = (TraceLevel) value;
            }
        }

        #endregion

        #region 内部フィールド

        /// <summary>
        ///     ログ出力スイッチ
        /// </summary>
        private static readonly TraceSwitch Sw = new TraceSwitch(LogFileIdentifier, HoI2Editor.Name);

        /// <summary>
        ///     ログファイル書き込み用
        /// </summary>
        private static StreamWriter _writer;

        /// <summary>
        ///     ログファイル書き込み用
        /// </summary>
        private static TextWriterTraceListener _listener;

        #endregion

        #region 内部定数

        /// <summary>
        ///     ログファイル名
        /// </summary>
        private const string LogFileName = "HoI2Editor.log";

        /// <summary>
        ///     ログファイル識別子
        /// </summary>
        private const string LogFileIdentifier = "HoI2EditorLog";

        #endregion

        #region 初期化

        /// <summary>
        ///     ログを初期化する
        /// </summary>
        public static void Init()
        {
            try
            {
                _writer = new StreamWriter(LogFileName, true, Encoding.UTF8) {AutoFlush = true};
                _listener = new TextWriterTraceListener(_writer, LogFileIdentifier);
                Trace.Listeners.Add(_listener);
            }
            catch (Exception)
            {
                MessageBox.Show(Resources.LogFileOpenError, HoI2Editor.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Terminate();
            }
            Verbose("[Log] Init");
        }

        /// <summary>
        ///     ログを終了する
        /// </summary>
        public static void Terminate()
        {
            Verbose("[Log] Terminate");
            if (_listener != null)
            {
                Trace.Listeners.Remove(_listener);
            }
            if (_writer != null)
            {
                _writer.Close();
            }
        }

        #endregion

        #region ログ出力

        /// <summary>
        ///     無効トークンエラーをログ出力する
        /// </summary>
        /// <param name="categoryName">カテゴリ名</param>
        /// <param name="token">トークン</param>
        /// <param name="lexer">字句解析器</param>
        public static void InvalidToken(string categoryName, Token token, TextLexer lexer)
        {
            Warning("[{0}] Invalid token: {1} ({2} L{3})",
                categoryName, ObjectHelper.ToString(token.Value), lexer.FileName, lexer.LineNo);
        }

        /// <summary>
        ///     セクション解析エラーをログ出力する
        /// </summary>
        /// <param name="categoryName">カテゴリ名</param>
        /// <param name="sectionName">セクション名</param>
        public static void InvalidSection(string categoryName, string sectionName)
        {
            Warning("[{0}] Parse failed: {1} section", categoryName, sectionName);
        }

        /// <summary>
        ///     範囲外エラーをログ出力
        /// </summary>
        /// <param name="categoryName">カテゴリ名</param>
        /// <param name="itemName">項目名</param>
        /// <param name="token">トークン</param>
        /// <param name="lexer">字句解析器</param>
        public static void OutOfRange(string categoryName, string itemName, Token token, TextLexer lexer)
        {
            Warning("[{0} Out of range: {1} at {2} ({3} L{4})",
                categoryName, ObjectHelper.ToString(token.Value), itemName, lexer.FileName, lexer.LineNo);
        }

        /// <summary>
        ///     エラーログを出力する
        /// </summary>
        /// <param name="s">対象文字列</param>
        /// <param name="args">パラメータ</param>
        public static void Error(string s, params object[] args)
        {
            WriteLine(TraceLevel.Error, s, args);
        }

        /// <summary>
        ///     エラーログを出力する
        /// </summary>
        /// <param name="s">対象文字列</param>
        public static void Error(string s)
        {
            WriteLine(TraceLevel.Error, s);
        }

        /// <summary>
        ///     警告ログを出力する
        /// </summary>
        /// <param name="s">対象文字列</param>
        /// <param name="args">パラメータ</param>
        public static void Warning(string s, params object[] args)
        {
            WriteLine(TraceLevel.Warning, s, args);
        }

        /// <summary>
        ///     警告ログを出力する
        /// </summary>
        /// <param name="s">対象文字列</param>
        public static void Warning(string s)
        {
            WriteLine(TraceLevel.Warning, s);
        }

        /// <summary>
        ///     情報ログを出力する
        /// </summary>
        /// <param name="s">対象文字列</param>
        /// <param name="args">パラメータ</param>
        public static void Info(string s, params object[] args)
        {
            WriteLine(TraceLevel.Info, s, args);
        }

        /// <summary>
        ///     情報ログを出力する
        /// </summary>
        /// <param name="s">対象文字列</param>
        public static void Info(string s)
        {
            WriteLine(TraceLevel.Info, s);
        }

        /// <summary>
        ///     詳細ログを出力する
        /// </summary>
        /// <param name="s">対象文字列</param>
        /// <param name="args">パラメータ</param>
        public static void Verbose(string s, params object[] args)
        {
            WriteLine(TraceLevel.Verbose, s, args);
        }

        /// <summary>
        ///     詳細ログを出力する
        /// </summary>
        /// <param name="s">対象文字列</param>
        public static void Verbose(string s)
        {
            WriteLine(TraceLevel.Verbose, s);
        }

        /// <summary>
        ///     ログを出力する
        /// </summary>
        /// <param name="level">ログ出力レベル</param>
        /// <param name="s">対象文字列</param>
        /// <param name="args">パラメータ</param>
        private static void WriteLine(TraceLevel level, string s, params object[] args)
        {
            bool condition;
            switch (level)
            {
                case TraceLevel.Error:
                    condition = Sw.TraceError;
                    break;

                case TraceLevel.Warning:
                    condition = Sw.TraceWarning;
                    break;

                case TraceLevel.Info:
                    condition = Sw.TraceInfo;
                    break;

                case TraceLevel.Verbose:
                    condition = Sw.TraceVerbose;
                    break;

                default:
                    return;
            }
            string t = string.Format(s, args);
            Trace.WriteLineIf(condition, t);
        }

        #endregion
    }
}