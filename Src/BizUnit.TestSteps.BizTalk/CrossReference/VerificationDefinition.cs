using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace BizUnit.TestSteps.BizTalk.CrossReference
{
    public class VerificationDefinition
    {

        private Collection<AppXRef> ids;
        private Collection<AppXRef> values;

        /// <summary>
        /// The Id to varify
        /// </summary>
        public Collection<AppXRef> Ids
        {
            get
            {
                return ids;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Ids cannot be null.");
                ids = value;
            }
        }

        /// <summary>
        /// The expected value
        /// </summary>
        public Collection<AppXRef> Values
        { 
            get
            {
                return values;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Values cannot be null.");
                values = value;
            }
        }
    }

    /// <summary>
    /// Modeled after original source.
    /// <VerifyId appInstance="application1" idXRef="Customer">25</VerifyId> 
    ///	<VerifyValue appType="application2" idXRef="Customer">12LK</VerifyValue>
    /// </summary>
    public class AppXRef
    {

        private string app;
        private string idXRef;
        private string val;

        public string App {
            get
            {
                return this.app;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("App value cannot be null.");
                this.app = value;
            }
        
        }
        public string IdXRef
        {
            get
            {
                return this.idXRef;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("IdXRef value cannot be null.");
                this.idXRef = value;
            }
        }

        public string Val
        {
            get
            {
                return this.val;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("Val value cannot be null.");
                this.val = value;
            }
        }
    }
}
