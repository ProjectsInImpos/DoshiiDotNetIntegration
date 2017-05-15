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
        /// </summary>
        /// <param name="updatedMember"></param>
        /// <returns></returns>
        void UpdateMemberOnPos(MemberOrg updatedMember);

        /// <summary>
        /// This method should delete a doshii member of the pos. 
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        bool DeleteMemberOnPos(string memberId);

        
    }
}
