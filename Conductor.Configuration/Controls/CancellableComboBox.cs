﻿using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Conductor.Configuration
{
 
    public class CancellableComboBox : ComboBox
    {
        public event CancelEventHandler BeforeUpdate;

        public CancellableComboBox()
        {
            this.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private bool mBusy;
        private int mPrevIndex = -1;

        protected virtual void OnBeforeUpdate(CancelEventArgs cea)
        {
            if (BeforeUpdate != null) BeforeUpdate(this, cea);
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            if (mBusy) return;
            mBusy = true;
            try
            {
                CancelEventArgs cea = new CancelEventArgs();
                OnBeforeUpdate(cea);
                if (cea.Cancel)
                {
                    // Restore previous index
                    this.SelectedIndex = mPrevIndex;
                    return;
                }
                mPrevIndex = this.SelectedIndex;
                base.OnSelectedIndexChanged(e);
            }
            finally
            {
                mBusy = false;
            }
        }
    }
}
