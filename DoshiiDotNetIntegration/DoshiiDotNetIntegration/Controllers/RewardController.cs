using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.CommunicationLogic;
using DoshiiDotNetIntegration.Enums;
using DoshiiDotNetIntegration.Exceptions;
using DoshiiDotNetIntegration.Helpers;
using DoshiiDotNetIntegration.Interfaces;
using DoshiiDotNetIntegration.Models;
using DoshiiDotNetIntegration.Models.ActionResults;
using NUnit.Framework;

namespace DoshiiDotNetIntegration.Controllers
{
    /// <summary>
    /// This class is used internally by the SDK to manage the SDK to manage the business logic handling memberships and rewards.
    /// </summary>
    internal class RewardController
    {
        /// <summary>
        /// prop for the local <see cref="ControllersCollection"/> instance. 
        /// </summary>
        internal Models.ControllersCollection _controllersCollection;

        /// <summary>
        /// prop for the local <see cref="HttpController"/> instance.
        /// </summary>
        internal HttpController _httpComs;

        /// <summary>
        /// constructor 
        /// </summary>
        /// <param name="controllerCollection"></param>
        /// <param name="httpComs"></param>
        internal RewardController(Models.ControllersCollection controllerCollection, HttpController httpComs)
        {
            if (controllerCollection == null)
            {
                throw new NullReferenceException("controller cannot be null");
            }
            _controllersCollection = controllerCollection;
            if (_controllersCollection.LoggingController == null)
            {
                throw new NullReferenceException("doshiiLogger cannot be null");
            }
            if (_controllersCollection.OrderingController == null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(OrderingController), DoshiiLogLevels.Fatal, " Initialization failed - IOrderingManager cannot be null");
                throw new NullReferenceException("orderingManager cannot be null");
            }
            if (_controllersCollection.RewardManager == null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(OrderingController), DoshiiLogLevels.Fatal, " Initialization failed - rewardManager cannot be null");
                throw new NullReferenceException("rewardManager cannot be null");
            }
            if (httpComs == null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(TransactionController), DoshiiLogLevels.Fatal, " Initialization failed - httpComs cannot be null");
                throw new NullReferenceException("httpComs cannot be null");
            }
            _httpComs = httpComs;
            
        }

        /// <summary>
        /// gets a member from doshii represented by the provided memberId. 
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        internal virtual ObjectActionResult<MemberOrg> GetMember(string memberId)
        {
            try
            {
                return _httpComs.GetMember(memberId);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// gets all the members from doshii for the orginisation. 
        /// </summary>
        /// <returns></returns>
        internal virtual ObjectActionResult<List<MemberOrg>> GetMembers()
        {
            try
            {
                return _httpComs.GetMembers();
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// deletes a member from doshii
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        internal virtual ActionResultBasic DeleteMember(string memberId)
        {
            if (string.IsNullOrEmpty(memberId))
            {
                _controllersCollection.LoggingController.mLog.LogDoshiiMessage(this.GetType(), DoshiiLogLevels.Warning, DoshiiStrings.GetAttemptingActionWithEmptyId("delete member", "member"));
                return new ActionResultBasic()
                {
                    Success = false,
                    FailReason = "member Id is empty"
                };
            }
            try
            {
                return _httpComs.DeleteMember(memberId);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// updates a member on Doshii
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        internal virtual ObjectActionResult<MemberOrg> UpdateMember(MemberOrg member)
        {
            if (string.IsNullOrEmpty(member.Name))
            {
                throw new MemberIncompleteException("member name is blank");
            }
            try
            {
                if (string.IsNullOrEmpty(member.Id))
                {
                    return _httpComs.PostMember(member);
                }
                else
                {
                    return _httpComs.PutMember(member);
                }

            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// syncs the pos members with the Doshii members, 
        /// NOTE: members that exist on the pos but do not exist on doshii will be deleted from the pos with a call to <see cref="IRewardManager.DeleteMemberOnPos"/>
        /// </summary>
        /// <returns>
        /// This method will return a failed result when any of the apps that are to be syncronised fail in the sync process, the fail reason in the ActionResultBasic will 
        /// give details about which apps failed to sync. 
        /// </returns>
        internal virtual ActionResultBasic SyncDoshiiMembersWithPosMembers()
        {
            try
            {
                StringBuilder failedReasonBuilder = new StringBuilder();
                List<MemberOrg> DoshiiMembersList = GetMembers().ReturnObject;
                List<MemberOrg> PosMembersList = _controllersCollection.RewardManager.GetMembersFromPos().ToList();

                var doshiiMembersHashSet = new HashSet<string>(DoshiiMembersList.Select(p => p.Id));
                var posMembersHashSet = new HashSet<string>(PosMembersList.Select(p => p.Id));

                var membersNotInDoshii = PosMembersList.Where(p => !doshiiMembersHashSet.Contains(p.Id));
                foreach (var mem in membersNotInDoshii)
                {
                    try
                    {
                        _controllersCollection.RewardController.UpdateMember(mem);
                    }
                    catch (Exception ex)
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format(" There was an exception deleting a member on the pos with doshii memberId {0}", mem.Id), ex);
                        failedReasonBuilder.AppendLine(string.Format("member with Id {0} failed to delete from the pos",
                            mem.Id));
                    }
                   
                }

                var membersInPos = DoshiiMembersList.Where(p => posMembersHashSet.Contains(p.Id));
                foreach (var mem in membersInPos)
                {
                    MemberOrg posMember = PosMembersList.FirstOrDefault(p => p.Id == mem.Id);
                    if (!mem.Equals(posMember))
                    {
                        try
                        {
                            _controllersCollection.RewardManager.UpdateMemberOnPos(mem);
                        }
                        catch (Exception ex)
                        {
                            _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format(" There was an exception updating a member on the pos with doshii memberId {0}", mem.Id), ex);
                            failedReasonBuilder.AppendLine(string.Format("member with Id {0} failed to update on the pos",
                            mem.Id));
                        }
                        
                    }
                }

                var membersNotInPos = DoshiiMembersList.Where(p => !posMembersHashSet.Contains(p.Id));
                foreach (var mem in membersNotInPos)
                {
                    try
                    {
                        _controllersCollection.RewardManager.CreateMemberOnPos(mem);
                    }
                    catch(Exception ex)
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format(" There was an exception creating a member on the pos with doshii memberId {0}", mem.Id), ex);
                        failedReasonBuilder.AppendLine(string.Format("member with Id {0} failed to create on the pos",
                            mem.Id));
                    }

                }

                if (string.IsNullOrEmpty(failedReasonBuilder.ToString()))
                {
                    return new ActionResultBasic()
                    {
                        Success = true
                    };
                }
                else
                {
                    return new ActionResultBasic()
                    {
                        Success = false,
                        FailReason = failedReasonBuilder.ToString()
                    };
                }
            }
            catch (Exception ex)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format(" There was an exception while attempting to sync Doshii members with the pos"), ex);
                return new ActionResultBasic()
                {
                    Success = false,
                    FailReason = DoshiiStrings.GetThereWasAnExceptionSeeLogForDetails("syncing members")
                };
            }
        }

        /// <summary>
        /// get all the rewards for a member existing on Doshii, 
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="orderId"></param>
        /// <param name="orderTotal"></param>
        /// <returns></returns>
        internal virtual ObjectActionResult<List<Reward>> GetRewardsForMember(string memberId)
        {
            try
            {
                return _httpComs.GetRewardsForMember(memberId);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// attempts to redeem a reward from doshii
        /// </summary>
        /// <param name="member"></param>
        /// <param name="reward"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        internal virtual ActionResultBasic RedeemRewardForMember(MemberOrg member, Reward reward, Order order)
        {
            try
            {
                var returnedOrder = _controllersCollection.OrderingController.UpdateOrder(order);
                if (returnedOrder.ReturnObject == null)
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format(" The Order was not successfully sent to Doshii so the reward could not be redeemed."));
                    return new ActionResultBasic()
                    {
                        Success = false,
                        FailReason = returnedOrder.FailReason
                    };
                }
            }
            catch (Exception ex)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format(" There was an exception putting and Order to Doshii for a rewards redeem"), ex);
                throw new OrderUpdateException(ex.ToString());
            }
            try
            {
                return _httpComs.RedeemRewardForMember(member.Id, reward.Id, order);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// cancels the redemption of a reward from Doshii
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="rewardId"></param>
        /// <param name="cancelReason"></param>
        /// <returns></returns>
        internal virtual ActionResultBasic RedeemRewardForMemberCancel(string memberId, string rewardId, string cancelReason)
        {
            try
            {
                return _httpComs.RedeemRewardForMemberCancel(memberId, rewardId, cancelReason);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// confirms the redemption of a reward from Doshii
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="rewardId"></param>
        /// <returns></returns>
        internal virtual ActionResultBasic RedeemRewardForMemberConfirm(string memberId, string rewardId)
        {
            try
            {
                return _httpComs.RedeemRewardForMemberConfirm(memberId, rewardId);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// attempts to redeem points for a member on Doshii
        /// </summary>
        /// <param name="member"></param>
        /// <param name="app"></param>
        /// <param name="order"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        internal virtual ActionResultBasic RedeemPointsForMember(MemberOrg member, App app, Order order, int points)
        {
            try
            {
                var returnedOrderResult = _controllersCollection.OrderingController.UpdateOrder(order);
                if (returnedOrderResult.ReturnObject == null)
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format(" There was a problem updating the Order on Doshii, so the points can't re redeemed."));
                    return new ActionResultBasic()
                    {
                        Success = false,
                        FailReason = returnedOrderResult.FailReason
                    };
                }
            }
            catch (Exception ex)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format(" There was an exception putting and Order to Doshii for a rewards redeem"), ex);
                throw new OrderUpdateException(ex.ToString());
            }
            PointsRedeem pr = new PointsRedeem()
            {
                AppId = app.Id,
                OrderId = order.Id,
                Points = points
            };
            try
            {
                return _httpComs.RedeemPointsForMember(pr, member);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// confirms the redemption of points for a member on Doshii
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        internal virtual ActionResultBasic RedeemPointsForMemberConfirm(string memberId)
        {
            try
            {
                return _httpComs.RedeemPointsForMemberConfirm(memberId);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// cancles the redemption of points for a member on Doshii. 
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="cancelReason"></param>
        /// <returns></returns>
        public virtual ActionResultBasic RedeemPointsForMemberCancel(string memberId, string cancelReason)
        {
            try
            {
                return _httpComs.RedeemPointsForMemberCancel(memberId, cancelReason);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }
    }
}
