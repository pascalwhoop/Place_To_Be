using placeToBe.Model.Entities;
using placeToBe.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace placeToBe.Services
{
    public class ProvideFbUserService
    {
        FbUserRepository fbUserRepo = new FbUserRepository();
        public List<Datum> data { get; set; }

        public async Task <List<FbUser>> getEventAttendingFriends(FbUser fbUser, Event currentEvent)
        {
            List<FbUser> eventAttendingFriends = new List<FbUser>();
            List<Datum> fbUserFriends = new List<Datum>();
            List<Rsvp> eventAttendingPeople = currentEvent.attending;

            fbUserFriends = fbUser.friends.data;

            for (int i = 0; i < fbUserFriends.Count; i++)
            {
               for(int j=0; j<eventAttendingPeople.Count; j++){
                if(fbUserFriends.ElementAt(i).id==eventAttendingPeople.ElementAt(j).id)
                    eventAttendingFriends.Add(await fbUserRepo.GetByFbIdAsync(fbUserFriends.ElementAt(i).id));
               }
            }
                return eventAttendingFriends;
        }

        public void setFbUserFriends(String fbUserId, String accessUserToken)
        {
            //ToDoIfNeeded server side call of fbUser friends who also use placeToBe
        }

    }
}