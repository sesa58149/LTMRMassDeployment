﻿#pragma checksum "..\..\mbWriteWin.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "F43D5C1195B131A4614C40C3D5B0C715"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using ftpMassUpgrade;


namespace ftpMassUpgrade {
    
    
    /// <summary>
    /// mbWriteWin
    /// </summary>
    public partial class mbWriteWin : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 10 "..\..\mbWriteWin.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtVal;
        
        #line default
        #line hidden
        
        
        #line 11 "..\..\mbWriteWin.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtValHex;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\mbWriteWin.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblAddress;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\mbWriteWin.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btModbusAccessWin;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\mbWriteWin.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btSendCmd;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\mbWriteWin.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtmbAdd;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\mbWriteWin.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblAddress_Copy;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\mbWriteWin.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblAddress_Copy1;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\mbWriteWin.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblAddress_Copy2;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\mbWriteWin.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblSuccessCode;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/ftpMassUpgrade;component/mbwritewin.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\mbWriteWin.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.txtVal = ((System.Windows.Controls.TextBox)(target));
            
            #line 10 "..\..\mbWriteWin.xaml"
            this.txtVal.LostKeyboardFocus += new System.Windows.Input.KeyboardFocusChangedEventHandler(this.txtEnterValDone_KBLostFocus);
            
            #line default
            #line hidden
            
            #line 10 "..\..\mbWriteWin.xaml"
            this.txtVal.KeyUp += new System.Windows.Input.KeyEventHandler(this.txtEnterChar_KBCharChange);
            
            #line default
            #line hidden
            return;
            case 2:
            this.txtValHex = ((System.Windows.Controls.TextBox)(target));
            
            #line 11 "..\..\mbWriteWin.xaml"
            this.txtValHex.LostKeyboardFocus += new System.Windows.Input.KeyboardFocusChangedEventHandler(this.txtHexEnterValDone_KBLostFocus);
            
            #line default
            #line hidden
            return;
            case 3:
            this.lblAddress = ((System.Windows.Controls.Label)(target));
            return;
            case 4:
            this.btModbusAccessWin = ((System.Windows.Controls.Button)(target));
            
            #line 13 "..\..\mbWriteWin.xaml"
            this.btModbusAccessWin.Click += new System.Windows.RoutedEventHandler(this.btModbusAccessWin_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.btSendCmd = ((System.Windows.Controls.Button)(target));
            
            #line 14 "..\..\mbWriteWin.xaml"
            this.btSendCmd.Click += new System.Windows.RoutedEventHandler(this.btSendCmd_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.txtmbAdd = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            this.lblAddress_Copy = ((System.Windows.Controls.Label)(target));
            return;
            case 8:
            this.lblAddress_Copy1 = ((System.Windows.Controls.Label)(target));
            return;
            case 9:
            this.lblAddress_Copy2 = ((System.Windows.Controls.Label)(target));
            return;
            case 10:
            this.lblSuccessCode = ((System.Windows.Controls.Label)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
