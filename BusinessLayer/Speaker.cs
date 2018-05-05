using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer
{
    /// <summary>
    /// Represents a single speaker
    /// </summary>
    public class Speaker
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int? YearsExperience { get; set; }
        public bool HasBlog { get; set; }
        public string BlogURL { get; set; }
        public WebBrowser Browser { get; set; }
        public List<string> Certifications { get; set; }
        public string Employer { get; set; }
        public int RegistrationFee { get; set; }
        public List<BusinessLayer.Session> Sessions { get; set; }

        public readonly int[] MINIMUM_YEARS_EXPERIENCE = { 0, 2, 4, 6 };
        public readonly int[] MAXIMUM_YEARS_EXPERIENCE = { 1, 3, 5, 9 };
        public readonly int[] REGISTRATION_FEE_VALUE = { 500, 250, 100, 50 };


        /// <summary>
        /// Register a speaker
        /// </summary>
        /// <returns>speakerID</returns>
        public int? Register(IRepository repository)
        {
            //lets init some vars
            int? speakerId = null;

            ValidateRegistration();
            CalculateRegistrationFee();
            speakerId = SaveSpeakerToDB(repository);

            //if we got this far, the speaker is registered.
            return speakerId;
        }

        public void ValidateRegistration()
        {
            ValidateDataEmptyAndSession();
            if (HasRequirementsComplete() || !IsInDomainOrBrowser())
            {
                ApprovedSession();
            }
            else
            {
                throw new SpeakerDoesntMeetRequirementsException("Speaker doesn't meet our abitrary and capricious standards.");
            }
        }

        public void ValidateDataEmptyAndSession()
        {
            if (string.IsNullOrWhiteSpace(FirstName)) throw new ArgumentNullException("First Name is required");
            if (string.IsNullOrWhiteSpace(LastName)) throw new ArgumentNullException("Last name is required.");
            if (string.IsNullOrWhiteSpace(Email)) throw new ArgumentNullException("Email is required.");
            if (Sessions.Count() == 0) throw new ArgumentException("Can't register speaker with no sessions to present.");

        }

        public bool HasRequirementsComplete()
        {
            if (YearsExperience > 10) return true;
            if (HasBlog) return true;
            if (Certifications.Count() > 3) return true;
            if (IsInEmployerList()) return true;

            return false;
        }

        public bool IsInEmployerList()
        {
            var employerList = new List<string>() { "Microsoft", "Google", "Fog Creek Software", "37Signals" };
            return employerList.Contains(Employer);
        }

        public bool IsInDomainOrBrowser()
        {
            return IsInDomainList() || IsBrowserValid();
        }

        public bool IsInDomainList()
        {
            string emailDomain = Email.Split('@').Last();
            var domainsList = new List<string>() { "aol.com", "hotmail.com", "prodigy.com", "CompuServe.com" };

            return domainsList.Contains(emailDomain);
        }

        public bool IsBrowserValid()
        {
            return Browser.Name == WebBrowser.BrowserName.InternetExplorer && Browser.MajorVersion < 9;
        }


        public void ApprovedSession()
        {
            //var newTechnologies = new List<string> {"MVC4", "Node.js", "CouchDB", "KendoUI", "Dapper", "Angular"};
            var oldTechnologies = new List<string>() { "Cobol", "Punch Cards", "Commodore", "VBScript" };
            bool isApprovedSession = false;
            foreach (var session in Sessions)
            {
                //foreach (var technologie in newTechnologies)
                //{
                //    if (session.Title.Contains(technologie))
                //    {
                //        session.Approved = true;
                //        break;
                //    }
                //}

                foreach (var technologie in oldTechnologies)
                {
                    if (session.Title.Contains(technologie) || session.Description.Contains(technologie))
                    {
                        session.Approved = false;
                        break;
                    }
                    else
                    {
                        session.Approved = true;
                        isApprovedSession = true;
                    }
                }
            }

            if (!isApprovedSession) throw new NoSessionsApprovedException("No sessions approved.");
        }



        public void CalculateRegistrationFee()
        {
            RegistrationFee = 0;
            for (int i = 0; i < MINIMUM_YEARS_EXPERIENCE.Length; i++)
            {
                if (MINIMUM_YEARS_EXPERIENCE[i] <= YearsExperience && YearsExperience <= MAXIMUM_YEARS_EXPERIENCE[i])
                {
                    RegistrationFee = REGISTRATION_FEE_VALUE[i];
                    break;
                }
            }
        }

        public int? SaveSpeakerToDB(IRepository repository)
        {
            int? speakerId = null;
            //Now, save the speaker and sessions to the db.
            try
            {
                speakerId = repository.SaveSpeaker(this);
            }
            catch (Exception e)
            {
                //in case the db call fails 
            }

            return speakerId;
        }



        #region Custom Exceptions
        public class SpeakerDoesntMeetRequirementsException : Exception
        {
            public SpeakerDoesntMeetRequirementsException(string message)
                : base(message)
            {
            }

            public SpeakerDoesntMeetRequirementsException(string format, params object[] args)
                : base(string.Format(format, args)) { }
        }

        public class NoSessionsApprovedException : Exception
        {
            public NoSessionsApprovedException(string message)
                : base(message)
            {
            }
        }
        #endregion
    }
}