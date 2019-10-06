using Gov.Meeting.Cities.Miami.CityCommissionMeeting.Sections.AttorneyClient;
using Gov.Meeting.Cities.Miami.CityCommissionMeeting.Sections.BoardsAndCommittee;
using Gov.Meeting.Cities.Miami.CityCommissionMeeting.Sections.ConsentAgenda;
using Gov.Meeting.Cities.Miami.CityCommissionMeeting.Sections.Discussion;
using Gov.Meeting.Cities.Miami.CityCommissionMeeting.Sections.District;
using Gov.Meeting.Cities.Miami.CityCommissionMeeting.Sections.FutureLegislation;
using Gov.Meeting.Cities.Miami.CityCommissionMeeting.Sections.MayoralVetoe;
using Gov.Meeting.Cities.Miami.CityCommissionMeeting.Sections.PublicHearing;
using Gov.Meeting.Cities.Miami.CityCommissionMeeting.Sections.Reading;
using Gov.Meeting.Cities.Miami.CityCommissionMeeting.Sections.Resolution;
using Gov.Meeting.Cities.Miami.CityCommissionMeeting.Sections.SecondReading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gov.Meeting.Cities.Miami.CityCommissionMeeting
{
    public class Meeting
    {
        #region Properties
        public ConsentAgenda ConsentAgenda { get; set; }
        public MayoralVetoe MayoralVetoe { get; set; }
        public PublicHearing PublicHearing { get; set; }
        public SecondReading SecondReading { get; set; }
        public Reading FirstReading { get; set; }
        public Resolution Resolution { get; set; }
        public AttorneyClientSession AttorneyClientSession { get; set; }
        public BoardAndCommittee BoardsAndCommittee { get; set; }
        public Discussion Discussion { get; set; }
        public District District3 { get; set; }
        public FutureLegislation FutureLegislationSection { get; set; }
        #endregion
    }
}
