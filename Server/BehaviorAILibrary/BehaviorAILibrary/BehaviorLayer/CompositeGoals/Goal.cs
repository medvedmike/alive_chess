///////////////////////////////////////////////////////////
//  Goal.cs
//  Implementation of the Class Goal
//  Generated by Enterprise Architect
//  Created on:      16-���-2009 0:07:57
///////////////////////////////////////////////////////////
using System;

namespace BehaviorAILibrary.BehaviorLayer
{
    public abstract class Goal : IDisposable
    {
        private BotKing botKing;
        private GoalStatuses status = GoalStatuses.Inactive;
        protected GoalStatuses Status
        {
            get 
            {
                return status;
            }
            set
            {
                status = value;
            }
        }
        protected void ActivateIfInactive()
        {
            if (!IsActivate())
            {
                Activate();
            }
        }
        protected void ReactivateIfFailed()
        {
            if (HasFailed())
            {
                Status = GoalStatuses.Inactive;
            }
        }
        public int Priority { get; set; }
        public Goal()
        {

        }
        public virtual void Dispose()
        {

        }
        /// 
        /// <param name="botKing"></param>
        public Goal(BotKing botKing)
        {

        }

        public abstract void Activate();

        /// 
        /// <param name="goal"></param>
        public virtual void AddSubGoal(Goal goal)
        {
            throw new InvalidOperationException();
        }

        public abstract GoalStatuses Process();        

        public abstract void Terminate();

        public virtual bool IsCompleted()
        {
            return Status == GoalStatuses.Completed ? true : false;
        }

        public virtual bool HasFailed()
        {
            return Status == GoalStatuses.Failed ? true : false;
        }

        public virtual bool IsActivate()
        {
            return Status == GoalStatuses.Active? true : false;
        }

    }//end Goal

}//end namespace BehaviorLayer