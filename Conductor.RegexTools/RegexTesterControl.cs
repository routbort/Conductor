using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using ScintillaNET;
using System.Diagnostics;
using System.Threading;

namespace Conductor.RegexTools
{

    public partial class RegexTesterControl : UserControl
    {
        public RegexTesterControl()
        {
            InitializeComponent();
            this.txtTestText.Dock = DockStyle.Fill;
            this.gridCaptures.CaptionLabel.Text = "Captures";
            this.txtTestText.UsePopup(false);

            ContextMenuStrip cms = new ContextMenuStrip();
            cms.ShowCheckMargin = false;
            cms.ShowImageMargin = false;
            _MenuItemAddCapture = new ToolStripMenuItem("Add line capture", null, _MenuItemAddCapture_Click, Keys.Control | Keys.A);

            _MenuItemAddCapture.Enabled = false;

            _MenuItemAddOptionalCapture = new ToolStripMenuItem("Add (optional) line capture", null, _MenuItemAddOptionalCapture_Click, Keys.Control | Keys.O);

            _MenuItemAddCapture.Enabled = false;

            cms.Opening += Cms_Opening;
            cms.Items.Add(_MenuItemAddCapture);
            cms.Items.Add(_MenuItemAddOptionalCapture);
            txtTestText.ContextMenuStrip = cms;
        }

        private void Cms_Opening(object sender, CancelEventArgs e)
        {
            string selectedText = txtTestText.SelectedText;
            string elementName = GetDataElementName(selectedText);
            _MenuItemAddCapture.Text = "Add capture group " + elementName;
            _MenuItemAddCapture.Enabled = (elementName != "");
        }

        private void _MenuItemAddCapture_Click(object sender, EventArgs e)
        {
            string fragment = GetSelectedCaptureDefinition();
            if (fragment != null)
                this.Pattern = this.Pattern + fragment;
        }

        private void _MenuItemAddOptionalCapture_Click(object sender, EventArgs e)
        {
            string fragment = GetSelectedCaptureDefinition();
            if (fragment != null)
                this.Pattern = this.Pattern + MakeOptional(fragment);

        }
        string GetSelectedCaptureDefinition()
        {
            string selectedText = txtTestText.SelectedText;
            if (selectedText == "")
                return null;

            string dataElementName = GetDataElementName(selectedText);
            string regexFragment = ".*" + selectedText.Replace("(", @"\(").Replace(")", @"\)");
            if (regexFragment.EndsWith(" "))
                regexFragment += "?";
            regexFragment += "(?<" + dataElementName + @">.*?)" + System.Environment.NewLine;
            //  regexFragment += "(?<" + dataElementName + @">.*?)" + System.Environment.NewLine;


            return regexFragment;
        }


        string GetSelectedOptionalCaptureDefinition()

        {
            string selectedText = txtTestText.SelectedText;
            if (selectedText == "")
                return null;

            string dataElementName = GetDataElementName(selectedText);
            string regexFragment = ".*" + selectedText.Replace("(", @"\(").Replace(")", @"\)");
            if (regexFragment.EndsWith(" "))
                regexFragment += "?";

            regexFragment = @"(?:\n" + regexFragment + @"(?<" + dataElementName + @">[^\n*))?+.*?";


            return regexFragment;
        }

        string MakeOptional(string pattern)
        {

            //      pattern = pattern + @"(?:\nToken" + i.ToString() + ": ?(?<Token" + i.ToString() + @">[^\n]*))?+.*?" ;
            //(?:\nToken1: ?(?<Token1>[^\n]*))?+.*?


            if (pattern == null) return null;
            return "(?:" + pattern + ")?";
        }



        const int STYLE_MAX = 30;
        private Conductor.RegexTools.SafeRegex _re = null;
        ToolStripMenuItem _MenuItemAddCapture = null;
        ToolStripMenuItem _MenuItemAddOptionalCapture = null;

        public string GetMatchingText() { return this.txtTestText.Text; }

        public string Status { get { return this.lblResultSummary.Text; } }

        bool _Dirty = true;
        public string Pattern

        {
            get { return this.txtRE.Text; }
            set { if (this.txtRE.Text != value) { _Dirty = true; this.txtRE.Text = value; MatchText(); } }
        }

        private string Escape(string pre)
        {

            return null;
        }
        private string Unescape(string pre)
        {

            return null;
        }



        public string Input

        {
            get { return this.txtTestText.Text; }
            set
            {
                if (this.txtTestText.Text != value)
                { this.txtTestText.Text = value; MatchText(); }
            }
        }


        private class Target
        {
            public string name { get; set; }

            public Target(string text)
            { this.name = text; }
            private Target() { }
        }


        private void SetInfo(string info, bool error = false)
        {
            this.lblResultSummary.Text = info;
            this.lblResultSummary.BackColor = (error) ? Color.Red : Color.Green;
            if (error)
            {
                this.gridCaptures.DataSource = null;
                _namedCaptures.Clear();
            }
        }

        private void SetPending()
        {
            this.lblResultSummary.Text = "Running...";
            this.lblResultSummary.BackColor = SystemColors.Control;
        }


        BindingList<Capture> _namedCaptures = new BindingList<Capture>();
        MatchCollection _matches = null;

        public void MatchText()
        {
            SetPending();
            this.Refresh();
            txtTestText.ClearDocumentStyle();
            txtTestText.StartStyling(0);
            txtTestText.SetStyling(txtTestText.TextLength, 0);

            txtTestText.Styles[Style.Default].Size = 12;
            txtTestText.Styles[Style.Default].Font = "Consolas";


            if (this.txtRE.Text == "" || this.txtTestText.Text == "")

            {
                this.lblResultSummary.Text = "Nothing to match";
                this.lblResultSummary.BackColor = SystemColors.Control;
                return;
            }

            if (_Dirty)
            {
                try
                {
                    _re = new RegexTools.SafeRegex(Pattern, 500, RegexOptions.Singleline | RegexOptions.ExplicitCapture);
                    _Dirty = false;
                }
                catch (Exception ex)
                {
                    SetInfo("Pattern error: " + ex.Message, true);
                    //  ResumeUpdating();
                    return;
                }
            }


            var groupNames = _re.BaseRegex.GetGroupNames();
            List<Target> targets = new List<Target>();
            foreach (var groupName in groupNames)
                if (groupName.ToString() != "0")
                {
                    targets.Add(new Target(groupName.ToString()));
                }
            this.lblResultSummary.Visible = true;
            string info = "";

            try
            {
                _matches = _re.Matches(Input);
            }
            catch (Conductor.RegexTools.RegexPerformanceException pex)
            {
                SetInfo("Performance error: " + pex.Message, true);
                return;
            }
            catch (Exception ex)
            {
                SetInfo("Error: " + ex.Message, true);
                return;
            }

            if (_matches.Count == 0)
                info = "No match";
            if (_matches.Count == 1)
                info = "Single match starting at " + _matches[0].Index;
            if (_matches.Count > 1)
                info = _matches.Count.ToString() + " matches";

            info = info + " (" + _re.LastMatchExecutionTime.Value + " ms)";

            SetInfo(info, _matches.Count == 0);
            _namedCaptures.Clear();
            int colorIndex = 0;
            for (int matchIndex = 0; matchIndex < _matches.Count; matchIndex++)
            {
                //   colorIndex++;
                Match match = _matches[matchIndex];
                //   List<Capture> currentCaptures = new List<Capture>();


                foreach (Target target in targets)
                {
                    Capture capture = new Capture(target.name);

                    Group g = match.Groups[target.name];

                    capture.value = g.Value;
                    capture.position = g.Index;
                    capture.length = g.Length;

                    _namedCaptures.Add(capture);
                    //  currentCaptures.Add(capture);

                }
                // allCap.Add(currentCaptures);

                for (int groupIndex = 0; groupIndex < match.Groups.Count; groupIndex++)
                {

                    colorIndex++;
                    Group group = match.Groups[groupIndex];




                    for (int captureIndex = 0; captureIndex < group.Captures.Count; captureIndex++)
                    {
                        System.Text.RegularExpressions.Capture capture = group.Captures[captureIndex];

                        //    System.Diagnostics.Debug.WriteLine(matchIndex.ToString() + ":" + groupIndex.ToString() + ":" + captureIndex.ToString() + "  " + match.Index.ToString() + ":" + group.Index.ToString() + ":" +
                        // capture.Index.ToString() + " '" + capture.Value + "'" + " ColorIndex:" + colorIndex.ToString());
                        colorIndex = colorIndex++ % STYLE_MAX;
                        if (colorIndex == 0)
                            colorIndex = 1;
                        if (false)
                        {
                            // if (colorIndex > 30) return;
                            int start = capture.Index;
                            txtTestText.StartStyling(start);
                            txtTestText.Styles[colorIndex].BackColor = Conductor.GUI.ColorHelper.GetColor(colorIndex);
                            int length = capture.Length;
                            txtTestText.SetStyling(length, colorIndex);




                        }
                    }
                }

            }

            colorIndex = -1;
            int styleIndex;

            Capture firstCapture = (_namedCaptures.Count > 0) ? _namedCaptures[0] : null;
            foreach (var capture in _namedCaptures)
            {
                colorIndex = (colorIndex + 1) % STYLE_MAX;
                styleIndex = colorIndex + 1;
                if (capture.position != 0)
                {
                    int start = capture.position;
                    txtTestText.StartStyling(start);
                    txtTestText.Styles[styleIndex].BackColor = Conductor.GUI.ColorHelper.GetColor(colorIndex);
                    int length = capture.length;
                    txtTestText.SetStyling(length, styleIndex);
                }
            }

            if (firstCapture != null)
            {
                int position = firstCapture.position;
                Line line = this.txtTestText.Lines[this.txtTestText.LineFromPosition(position)];
                //    this.txtTestText.ScrollRange(line.Position, line.Position);
            }
           
            this.gridCaptures.DataSource = _namedCaptures;
            this.gridCaptures.Grid.Columns["position"].Visible = false;
            this.gridCaptures.Grid.Columns["length"].Visible = false;
            //ResumeUpdating();
        }


        public void NavigateToFirstMatch()
        {
            if (_namedCaptures != null & _namedCaptures.Count > 0)
            {
                int position = _namedCaptures[0].position;
                var line = txtTestText.Lines[this.txtTestText.LineFromPosition(position)];
                txtTestText.FirstVisibleLine = this.txtTestText.LineFromPosition(position);
                return;
                var linesOnScreen = txtTestText.LinesOnScreen - 2; // Fudge factor

                this.txtTestText.ScrollRange(line.Position, txtTestText.Lines[line.Index + (linesOnScreen / 2)].Position);




                // var start = txtTestText.Lines[line - (linesOnScreen / 2)].Position;
                // var end = txtTestText.Lines[line + (linesOnScreen / 2)].Position;
                //  txtTestText.ScrollRange(start, end);



            }

        }


        public List<Capture> CurrentCaptures
        { get { return this._namedCaptures.ToList<Capture>(); } }

        public bool MatchFound
        {
            get
            {
                if (_re == null)
                    return false;
                return _re.MatchFound;
            }
        }

        public Dictionary<string, Capture> CurrentCapturesAsDict
        {
            get
            {
                Dictionary<string, Capture> results = new Dictionary<string, RegexTools.Capture>();
                foreach (Capture c in _namedCaptures)
                {
                    if (results.ContainsKey(c.name) && results[c.name].value != c.value)
                        throw new ApplicationException("More than one capture named " + c.name + " - with different values.  Cannot get dictionary.");
                    results[c.name] = c;
                }

                return results;
            }

        }

        private void txtTestText_TextChanged(object sender, EventArgs e)
        {
            MatchText();
        }

        private void txtRE_TextChanged(object sender, EventArgs e)
        {
            _Dirty = true;
            MatchText();
        }

        private string GetDataElementName(string input)
        {
            string selectedText = input;
            string captureName = selectedText.TrimEnd(_Space);
            captureName = captureName.Replace("%", "Percent ").Replace("(", "").Replace(")", "").Replace(":", "").Replace("-", " ").Replace(",", " ").Replace(".", " ");
            string[] pieces = captureName.Split(_Space, StringSplitOptions.RemoveEmptyEntries);
            string dataElementName = "";
            foreach (var piece in pieces)
                dataElementName = dataElementName + piece.Substring(0, 1).ToUpper() + piece.Substring(1);
            return dataElementName;
        }


        char[] _Space = new char[] { };

        private void button1_Click(object sender, EventArgs e)
        {
            string fragment = GetSelectedCaptureDefinition();
            if (fragment != null)
                this.Pattern = this.Pattern + fragment;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Pattern = Pattern.Replace(@"\r", Environment.NewLine);
        }

        private void txtTestText_UpdateUI(object sender, UpdateUIEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string selectedText = this.txtTestText.SelectedText;
            string pattern = "";
            string[] lines = selectedText.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {

                if (line.Contains(":"))
                {
                    string[] pieces = line.Split(new string[] { ": " }, StringSplitOptions.None);
                    if (pieces.Length == 2)
                    {
                        selectedText = pieces[0];
                        string dataElementName = GetDataElementName(selectedText);
                        string regexFragment = ".*" + selectedText.Replace("(", @"\(").Replace(")", @"\)") + ": ?";
                        regexFragment += "(?<" + dataElementName + @">.*?)" + System.Environment.NewLine;
                        pattern += regexFragment;
                    }
                }


            }
            this.Pattern = pattern;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string selectedText = this.txtTestText.SelectedText;
            string pattern = "";
            string[] lines = selectedText.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)

            {
                var line = lines[lineIndex];
                if (line.Contains(":"))
                {
                    string[] pieces = line.Split(new string[] { ": " }, StringSplitOptions.None);
                    if (pieces.Length == 2)
                    {
                        selectedText = pieces[0];
                        string dataElementName = GetDataElementName(selectedText);
                        string regexFragment = @"(?:.*?" + selectedText.Replace("(", @"\(").Replace(")", @"\)") + ": ?";
                        regexFragment += "(?<" + dataElementName + @">.*?)\n)?.*?";
                        pattern += regexFragment;
                    }
                }
                else
                {
                    pattern += line.Replace("(", @"\(").Replace(")", @"\)");
                    if (lineIndex != lines.Length - 1)
                        pattern += @".*?\r\n.*?";

                }
            }
            this.Pattern = pattern;

            this.txtTestText.SetEmptySelection(this.txtTestText.SelectionStart);

        }

        public void Clear()
        {
            this.lblResultSummary.Text = "";
            this.txtRE.Text = "";
            this.txtTestText.Text = "";
            this.gridCaptures.DataSource = null;
            _namedCaptures.Clear();
        }


        private void button5_Click(object sender, EventArgs e)
        {
            string regex = this.Pattern;
            string TextToMatch = this.Input;
            string LastSuccesfulRE = "";
            string LastSuccesfulFragment = "";

            string CurrentRE = "";
            string CurrentFragment = "";
            string BadFragment = "";
            string splitAt1 = @"\r\n";
            string splitAt2 = "?(?:";

            //string[] pieces = this.Pattern.Split(new string[] { "?(?:" }, StringSplitOptions.None);

            string[] pieces = this.Pattern.Split(new string[] { splitAt1 }, StringSplitOptions.None);

            string start = pieces[0];
            string finish = splitAt1 + pieces[pieces.Length - 1];
            var fragments = pieces.Skip(1).Take(pieces.Length - 2);
            string middle = "";
            bool success = false;
            foreach (var fragment in fragments)
            {
                middle += splitAt1 + fragment;

                CurrentFragment = fragment;
                string pattern = start + middle + finish;
                CurrentRE = pattern;

                SafeRegex re = new RegexTools.SafeRegex(pattern, 500, RegexOptions.Singleline | RegexOptions.ExplicitCapture);

                try
                {

                    MatchCollection mc = re.Matches(TextToMatch);
                    success = (mc.Count != 0);

                }
                catch
                {
                    success = false;

                }

                if (success)
                {
                    LastSuccesfulFragment = CurrentFragment;
                    LastSuccesfulRE = CurrentRE;
                }
                else
                {
                    BadFragment = CurrentFragment;
                    break;
                }

            }





            if (success)
                SetInfo("All match");
            else
            {

                string[] subpieces = CurrentFragment.Split(new string[] { splitAt2 }, StringSplitOptions.None);
                bool firstFragment = true;

                foreach (var subpiece in subpieces)
                {

                    if (firstFragment)
                    {
                        firstFragment = false;
                        middle += subpiece;
                    }
                    else
                        middle += splitAt2 + subpiece;

                    CurrentFragment = subpiece;
                    string pattern = start + middle + finish;
                    CurrentRE = pattern;

                    SafeRegex re = new RegexTools.SafeRegex(pattern, 500, RegexOptions.Singleline | RegexOptions.ExplicitCapture);

                    try
                    {

                        MatchCollection mc = re.Matches(TextToMatch);
                        success = (mc.Count != 0);

                    }
                    catch
                    {
                        success = false;
                    }

                    if (success)
                    {
                        LastSuccesfulFragment = CurrentFragment;
                        LastSuccesfulRE = CurrentRE;
                    }
                    else
                    {
                        BadFragment = CurrentFragment;
                        break;
                    }



                }



                string lastRe = LastSuccesfulRE;
                Pattern = LastSuccesfulRE;
                lblResultSummary.Text = "Bad frag:" + BadFragment;
            }

        }

        private void button6_Click(object sender, EventArgs e)
        {
            List<string> captureGroups = RegexHelper.GetNamedCaptureGroups(this.Pattern);
        }
    }


}