﻿using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using OmniSharp.Solution;
using OmniSharp.Common;

namespace OmniSharp.ProjectManipulation.AddToProject
{
    public class AddToProjectHandler
    {
        private readonly ISolution _solution;
        private readonly XNamespace _msBuildNameSpace = "http://schemas.microsoft.com/developer/msbuild/2003";
		private readonly IFileSystem _fileSystem;

        public AddToProjectHandler(ISolution solution, IFileSystem fileSystem)
        {
			_fileSystem = fileSystem;
            _solution = solution;
        }

        public void AddToProject(AddToProjectRequest request)
        {
            if (request.FileName == null || !request.FileName.EndsWith(".cs"))
            {
                return;
            }

            var relativeProject = _solution.ProjectContainingFile(request.FileName);

            if (relativeProject == null || relativeProject is OrphanProject)
            {
                throw new ProjectNotFoundException(string.Format("Unable to find project relative to file {0}", request.FileName));
            }

            var project = relativeProject.AsXml();

            var requestFile = request.FileName;
            var projectDirectory = _fileSystem.GetDirectoryName(relativeProject.FileName);
			var relativeFileName = requestFile.Replace(projectDirectory, "").ForceWindowsPathSeparator().Substring(1);

            var compilationNodes = project.Element(_msBuildNameSpace + "Project")
                                          .Elements(_msBuildNameSpace + "ItemGroup")
                                          .Elements(_msBuildNameSpace + "Compile").ToList();

            var fileAlreadyInProject = compilationNodes.Any(n => n.Attribute("Include").Value.Equals(relativeFileName, StringComparison.InvariantCultureIgnoreCase));

            if (!fileAlreadyInProject)
            {
                var compilationNodeParent = compilationNodes.First().Parent;

                var newFileElement = new XElement(_msBuildNameSpace + "Compile", new XAttribute("Include", relativeFileName));

                compilationNodeParent.Add(newFileElement);
                
                relativeProject.Save(project);
            }
        }
    }
}