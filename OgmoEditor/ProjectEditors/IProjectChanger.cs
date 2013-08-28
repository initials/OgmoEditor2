using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OgmoEditor.ProjectEditors
{
    public interface IProjectChanger
    {
        void LoadFromProject(Project project);
    }
}
