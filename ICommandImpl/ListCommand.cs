/*
*
* ListCommand.cs
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

using System.Windows.Input;

namespace ICommandImpl {

    public class ListCommand<T> : ICommand {
        private readonly Func<bool> canExecute;

        private readonly Func<IList<T>> func;

        public ListCommand(Func<bool> canExecute, Func<IList<T>> func) {
            this.canExecute = canExecute;
            this.func = func;
        }

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        #region ICommand

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => canExecute();

        public void Execute(object? parameter) => func();

        public IList<T> Execute() => func();

        #endregion ICommand
    }
}