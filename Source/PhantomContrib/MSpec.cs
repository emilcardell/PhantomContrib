using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phantom.Core.Builtins;
using Phantom.Core;

namespace PhantomContrib
{
    public class MSpec: ExecutableTool<MSpec> {
        public MSpec()
        {
            toolPath = "Library/MSpec/mspec.exe";
            teamCityArgs = "--teamcity";
            htmlReportArgs = "--html";
		}

		public string[] assemblies { get; set; }
		public string assembly { get; set; }
        public bool enableTeamCity { get; set; }
        public string teamCityArgs { get; set; }

        public string htmlReportPath { get; set; }
        public string htmlReportArgs { get; set; }

		protected override void Execute() {
			if ((assemblies == null || assemblies.Length == 0) && string.IsNullOrEmpty(assembly)) {
				throw new InvalidOperationException("Please specify either the 'assembly' or the 'assemblies' property when calling 'MSpec'");
			}

			//single assembly takes precedence.
			if (!string.IsNullOrEmpty(assembly)) {
				assemblies = new[] {assembly};
			}

			var args = new List<string>();

            if (enableTeamCity)
            {
                args.Add(teamCityArgs);
            }

            if(!string.IsNullOrEmpty(htmlReportPath))
            {
                args.Add(htmlReportArgs + " " + htmlReportPath);
            }


			foreach (var asm in assemblies) {
				var xunitArgs = new List<string>(args) {asm};
				Execute(xunitArgs.JoinWith(" "));
			}
		}
	}
}