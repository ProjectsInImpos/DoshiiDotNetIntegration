using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.Models;

namespace DoshiiDotNetIntegration.Interfaces
{
    /// <summary>
    /// Implementations of this interface is required to handle membership functionality in Doshii.
    /// <para/>The POS should implement this interface to enable member sales and rewards tracking.
    /// <para/>Version control on members is also managed through the POS implementation of this interface.
    /// </summary>
    /// <remarks>
    /// <para/><see cref="DoshiiController"/> uses this interface as a callback mechanism 
    /// to the POS for membership functions. 
    /// <para>
    /// </para>
    /// </remarks>
    public interface IRewardManager
    {
        /// <summary>
        /// This method should retreive the doshii member from the pos. 
        /// </summary>
        /// <param name="DoshiiMemberId"></param>
        /// <returns></returns>
        DoshiiDotNetIntegration.Models.MemberOrg RetrieveMemberFromPos(string DoshiiMemberId);

        /// <summary>
        /// This method should retreive all the doshii member from the pos. 
        /// </summary>
        /// <param name="DoshiiMemberId"></param>
        /// <returns></returns>
        IEnumerable<DoshiiDotNetIntegration.Models.MemberOrg> GetMembersFromPos();
        
        /// <summary>
        /// This method should create a doshii member on the pos
        /// </summary>
        /// <param name="newMember"></param>
        /// <returns></returns>
        void CreateMemberOnPos(MemberOrg newMember);

        /// <summary>
        /// This method should update a doshii member of the pos. 
        /// This will be called whenever a doshii member is updated in the cloud by either another locaiton in the orginisation or by a partner connected to the orginisation - for these calls the member.Id can be used to find the member on the pos to update. 
        /// </summary>
        /// <param name="updatedMember"></param>
        /// <returns></returns>
        void UpdateMemberOnPos(MemberOrg updatedMember);

        /// <summary>
        /// This method should update a doshii member on the pos. 
        /// The method will be called during the initial member sync when the sdk attempt to sync all members from the pos with doshii, when this is successful this method is called so the member can be updated by its email, as the pos is not aware of the id at this point. 
        /// This is the oportunity for the pos to save the id.  
        /// </summary>
        /// <param name="updatedMember"></param>
        void UpdateMemberOnPosByEmail(MemberOrg updatedMember);

        /// <summary>
        /// This method should delete a doshii member of the pos the memberId can be used to delete the member on the pos as this is a member that is already synced with doshii. 
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        bool DeleteMemberOnPos(string memberId);

        /// <summary>
        /// The method should delete a doshii member on the pos. 
        /// This method is called by the sdk when it attempt to sync members from the pos with doshii when the sdk is initialized
        /// At this point the member is not synced with doshii so does not have a member.Id so the email should be used to delete the member from the pos. 
        /// </summary>
        /// <param name="memberemail"></param>
        /// <returns></returns>
        bool DeleteMemberOnPosByEmail(string memberemail);
    }
}
