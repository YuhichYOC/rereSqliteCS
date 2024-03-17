/*
*
* VoidCommand.cs
*
* Copyright 2024 Yuichi Yoshii
*     吉井雄一 @ 吉井産業  you.65535.kir@gmail.com
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*     http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/

using System.Data;
using System.Windows.Input;

namespace ICommandImpl {

    public class VoidCommand : ICommand {
        private readonly Func<bool> canExecute;

        private readonly Action? action;

        private readonly Action<string>? sAction;

        private readonly Action<int, string, string, int, string, string, string>? cAction1;

        private readonly Action<int, string, DataTable, string, int, ICommand>? cAction2;

        private readonly Action<IVariantArg>? vAction;

        public VoidCommand(Func<bool> canExecute, Action action) {
            this.canExecute = canExecute;
            this.action = action;
        }

        public VoidCommand(Func<bool> canExecute, Action<string> action) {
            this.canExecute = canExecute;
            sAction = action;
        }

        public VoidCommand(Func<bool> canExecute, Action<int, string, string, int, string, string, string> action) {
            this.canExecute = canExecute;
            cAction1 = action;
        }

        public VoidCommand(Func<bool> canExecute, Action<int, string, DataTable, string, int, ICommand> action) {
            this.canExecute = canExecute;
            cAction2 = action;
        }

        public VoidCommand(Func<bool> canExecute, Action<IVariantArg> action) {
            this.canExecute = canExecute;
            vAction = action;
        }

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        #region ICommand

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => canExecute();

        public void Execute(object? parameter) => action?.Invoke();

        public void Execute() => action?.Invoke();

        public void Execute(string parameter) => sAction?.Invoke(parameter);

        public void Execute(int p1, string p2, string p3, int p4, string p5, string p6, string p7) => cAction1?.Invoke(p1, p2, p3, p4, p5, p6, p7);

        public void Execute(int p1, string p2, DataTable p3, string p4, int p5, ICommand p6) => cAction2?.Invoke(p1, p2, p3, p4, p5, p6);

        public void Execute(IVariantArg parameter) => vAction?.Invoke(parameter);

        #endregion ICommand
    }
}