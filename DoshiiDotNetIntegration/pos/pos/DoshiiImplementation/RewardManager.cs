using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.Interfaces;
using DoshiiDotNetIntegration.Models;

namespace pos.DoshiiImplementation
{
    public class RewardManager : IRewardManager
    {
        public MemberOrg RetrieveMemberFromPos(string DoshiiMemberId)
        {
            return LiveData.MemberList.FirstOrDefault(m => m.Id == DoshiiMemberId);
        }

        public IEnumerable<MemberOrg> GetMembersFromPos()
        {
            return LiveData.MemberList;
        }

        public void CreateMemberOnPos(MemberOrg newMember)
        {
            LiveData.MemberList.Add(newMember);
        }

        public void UpdateMemberOnPos(MemberOrg updatedMember)
        {
            foreach (var mem in LiveData.MemberList.Where(m => m.Id == updatedMember.Id))
            {
                mem.Apps = updatedMember.Apps;
                mem.Address = updatedMember.Address;
                mem.CreatedAt = updatedMember.CreatedAt;
                mem.UpdatedAt = updatedMember.UpdatedAt;
                mem.Email = updatedMember.Email;
                mem.FirstName = updatedMember.FirstName;
                mem.LastName = updatedMember.LastName;
                mem.Name = updatedMember.Name;
                mem.Phone = updatedMember.Phone;
                mem.Ref = updatedMember.Ref;
                mem.Uri = updatedMember.Uri;
            }
        }

        public bool DeleteMemberOnPos(string doshiiId)
        {
            try
            {
                LiveData.MemberList.RemoveAll(m => m.Id == doshiiId);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;

        }

        public void UpdateMemberOnPosByEmail(MemberOrg updatedMember)
        {
            foreach (var mem in LiveData.MemberList.Where(m => m.Email == updatedMember.Email))
            {
                mem.Apps = updatedMember.Apps;
                mem.Address = updatedMember.Address;
                mem.CreatedAt = updatedMember.CreatedAt;
                mem.UpdatedAt = updatedMember.UpdatedAt;
                mem.FirstName = updatedMember.FirstName;
                mem.LastName = updatedMember.LastName;
                mem.Name = updatedMember.Name;
                mem.Phone = updatedMember.Phone;
                mem.Ref = updatedMember.Ref;
                mem.Uri = updatedMember.Uri;
            }
        }

        public bool DeleteMemberOnPosByEmail(string memberemail)
        {
            try
            {
                LiveData.MemberList.RemoveAll(m => m.Email == memberemail);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}
