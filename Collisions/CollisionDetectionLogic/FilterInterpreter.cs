using System.Threading.Tasks.Dataflow;
using AlpimiAPI.Entities.EAvailability;
using AlpimiAPI.Entities.ECollision;
using AlpimiAPI.Entities.ECollisionType;
using AlpimiAPI.Entities.EDayOff;
using AlpimiAPI.Entities.ELessonBlock;
using AlpimiAPI.Entities.EScheduleSettings;
using alpimi_planner_backend.Collisions.CollisionDataObjects;
using alpimi_planner_backend.Collisions.CollisionUtils;
using alpimi_planner_backend.Collisions.RuleLibrary;
using Microsoft.AspNetCore.Components.Web;

namespace alpimi_planner_backend.Collisions.CollisionDetectionLogic
{
    public class FilterInterpreter
    {
        public static List<CollisionBase> Interpret(
            CollisionTypeData collisionTypeData,
            CollisionLessonBlock? lessonBlock,
            DateOnly scanDate,
            CollisionScanBlocks collisionScanBlocks,
            ScheduleSettings scheduleSettings,
            CollisionScanStaticData collisionScanStaticData,
            LogMaker logMaker
        )
        {
            List<Rule> objectRules = RuleTranslator.GetObjectRules(
                collisionTypeData.filter.objectRules
            );

            List<Rule> targetRules = RuleTranslator.GetTargetRules(
                collisionTypeData.filter.targetRules
            );

            List<Rule> methodRules = RuleTranslator.GetMethodRules(
                collisionTypeData.filter.methodRules
            );

            List<CollisionBase> collisions = new List<CollisionBase>();

            switch (collisionTypeData.category)
            {
                case "lessonblock":
                    if (lessonBlock != null)
                    {
                        bool ruleCheck = true;
                        foreach (Rule rule in objectRules)
                        {
                            ruleCheck = rule.CheckBlock(
                                lessonBlock,
                                scanDate,
                                collisionScanBlocks,
                                scheduleSettings,
                                collisionScanStaticData,
                                logMaker
                            );
                            if (!ruleCheck)
                            {
                                break;
                            }
                        }
                        if (ruleCheck)
                        {
                            foreach (Rule rule in methodRules)
                            {
                                ruleCheck = rule.CheckBlock(
                                    lessonBlock,
                                    scanDate,
                                    collisionScanBlocks,
                                    scheduleSettings,
                                    collisionScanStaticData,
                                    logMaker
                                );
                                if (!ruleCheck)
                                {
                                    break;
                                }
                            }
                            if (ruleCheck)
                            {
                                collisions.Add(
                                    new CollisionBase
                                    {
                                        Id = Guid.NewGuid(),
                                        CollidingObject1 = lessonBlock.Id.ToString(),
                                        CollidingObject2 = null,
                                        Ignored = false,
                                        CollisionTypeId = collisionTypeData.id
                                    }
                                );
                            }
                        }
                    }
                    break;
                case "lessonblock-lessonblock":
                    if (lessonBlock != null)
                    {
                        foreach (CollisionLessonBlock block in collisionScanBlocks.lessonBlocks)
                        {
                            if (
                                lessonBlock.LessonDate == block.LessonDate
                                && lessonBlock.Id != block.Id
                            )
                            {
                                CollisionLessonBlock objectBlock = lessonBlock;
                                CollisionLessonBlock targetBlock = block;
                                bool ruleCheckObject = true;
                                bool ruleCheckTarget = true;

                                foreach (Rule rule in objectRules)
                                {
                                    ruleCheckObject = rule.CheckBlock(
                                        objectBlock,
                                        scanDate,
                                        collisionScanBlocks,
                                        scheduleSettings,
                                        collisionScanStaticData,
                                        logMaker
                                    );
                                    if (!ruleCheckObject)
                                    {
                                        break;
                                    }
                                }

                                foreach (Rule rule in targetRules)
                                {
                                    ruleCheckTarget = rule.CheckBlock(
                                        targetBlock,
                                        scanDate,
                                        collisionScanBlocks,
                                        scheduleSettings,
                                        collisionScanStaticData,
                                        logMaker
                                    );
                                    if (!ruleCheckTarget)
                                    {
                                        break;
                                    }
                                }

                                if (ruleCheckObject && ruleCheckTarget)
                                {
                                    bool ruleCheck = true;

                                    foreach (Rule rule in methodRules)
                                    {
                                        ruleCheck = rule.CheckPair(
                                            objectBlock,
                                            targetBlock,
                                            scanDate,
                                            collisionScanBlocks,
                                            scheduleSettings,
                                            collisionScanStaticData,
                                            logMaker
                                        );
                                        if (!ruleCheck)
                                        {
                                            break;
                                        }
                                    }

                                    if (ruleCheck)
                                    {
                                        string collidingObject1;
                                        string collidingObject2;

                                        if (objectBlock.Id.CompareTo(targetBlock.Id) < 0)
                                        {
                                            collidingObject1 = objectBlock.Id.ToString();
                                            collidingObject2 = targetBlock.Id.ToString();
                                        }
                                        else
                                        {
                                            collidingObject1 = targetBlock.Id.ToString();
                                            collidingObject2 = objectBlock.Id.ToString();
                                        }
                                        collisions.Add(
                                            new CollisionBase
                                            {
                                                Id = Guid.NewGuid(),
                                                CollidingObject1 = collidingObject1,
                                                CollidingObject2 = collidingObject2,
                                                Ignored = false,
                                                CollisionTypeId = collisionTypeData.id
                                            }
                                        );
                                    }
                                }
                                else
                                {
                                    objectBlock = block;
                                    targetBlock = lessonBlock;

                                    ruleCheckObject = true;
                                    ruleCheckTarget = true;

                                    foreach (Rule rule in objectRules)
                                    {
                                        ruleCheckObject = rule.CheckBlock(
                                            objectBlock,
                                            scanDate,
                                            collisionScanBlocks,
                                            scheduleSettings,
                                            collisionScanStaticData,
                                            logMaker
                                        );
                                        if (!ruleCheckObject)
                                        {
                                            break;
                                        }
                                    }

                                    foreach (Rule rule in targetRules)
                                    {
                                        ruleCheckTarget = rule.CheckBlock(
                                            targetBlock,
                                            scanDate,
                                            collisionScanBlocks,
                                            scheduleSettings,
                                            collisionScanStaticData,
                                            logMaker
                                        );
                                        if (!ruleCheckTarget)
                                        {
                                            break;
                                        }
                                    }

                                    if (ruleCheckObject && ruleCheckTarget)
                                    {
                                        bool ruleCheck = true;

                                        foreach (Rule rule in methodRules)
                                        {
                                            ruleCheck = rule.CheckPair(
                                                objectBlock,
                                                targetBlock,
                                                scanDate,
                                                collisionScanBlocks,
                                                scheduleSettings,
                                                collisionScanStaticData,
                                                logMaker
                                            );
                                            if (!ruleCheck)
                                            {
                                                break;
                                            }
                                        }

                                        if (ruleCheck)
                                        {
                                            string collidingObject1;
                                            string collidingObject2;

                                            if (objectBlock.Id.CompareTo(targetBlock.Id) < 0)
                                            {
                                                collidingObject1 = objectBlock.Id.ToString();
                                                collidingObject2 = targetBlock.Id.ToString();
                                            }
                                            else
                                            {
                                                collidingObject1 = targetBlock.Id.ToString();
                                                collidingObject2 = objectBlock.Id.ToString();
                                            }
                                            collisions.Add(
                                                new CollisionBase
                                                {
                                                    Id = Guid.NewGuid(),
                                                    CollidingObject1 = collidingObject1,
                                                    CollidingObject2 = collidingObject2,
                                                    Ignored = false,
                                                    CollisionTypeId = collisionTypeData.id
                                                }
                                            );
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
                case "lessonblock-day":
                    if (lessonBlock != null)
                    {
                        foreach (CollisionLessonBlock block in collisionScanBlocks.lessonBlocks)
                        {
                            if (
                                Math.Abs(
                                    (
                                        lessonBlock.LessonDate.ToDateTime(TimeOnly.MinValue)
                                        - lessonBlock.LessonDate.ToDateTime(TimeOnly.MinValue)
                                    ).Days
                                ) <= 1
                                && lessonBlock.Id != block.Id
                            )
                            {
                                CollisionLessonBlock objectBlock = lessonBlock;
                                CollisionLessonBlock targetBlock = block;
                                bool ruleCheckObject = true;
                                bool ruleCheckTarget = true;

                                foreach (Rule rule in objectRules)
                                {
                                    ruleCheckObject = rule.CheckBlock(
                                        objectBlock,
                                        scanDate,
                                        collisionScanBlocks,
                                        scheduleSettings,
                                        collisionScanStaticData,
                                        logMaker
                                    );
                                    if (!ruleCheckObject)
                                    {
                                        break;
                                    }
                                }

                                foreach (Rule rule in targetRules)
                                {
                                    ruleCheckTarget = rule.CheckBlock(
                                        targetBlock,
                                        scanDate,
                                        collisionScanBlocks,
                                        scheduleSettings,
                                        collisionScanStaticData,
                                        logMaker
                                    );
                                    if (!ruleCheckTarget)
                                    {
                                        break;
                                    }
                                }

                                if (ruleCheckObject && ruleCheckTarget)
                                {
                                    bool ruleCheck = true;

                                    foreach (Rule rule in methodRules)
                                    {
                                        ruleCheck = rule.CheckPair(
                                            objectBlock,
                                            targetBlock,
                                            scanDate,
                                            collisionScanBlocks,
                                            scheduleSettings,
                                            collisionScanStaticData,
                                            logMaker
                                        );
                                        if (!ruleCheck)
                                        {
                                            break;
                                        }
                                    }

                                    if (ruleCheck)
                                    {
                                        string collidingObject1;
                                        string collidingObject2;

                                        if (objectBlock.Id.CompareTo(targetBlock.Id) < 0)
                                        {
                                            collidingObject1 = objectBlock.Id.ToString();
                                            collidingObject2 = targetBlock.Id.ToString();
                                        }
                                        else
                                        {
                                            collidingObject1 = targetBlock.Id.ToString();
                                            collidingObject2 = objectBlock.Id.ToString();
                                        }
                                        collisions.Add(
                                            new CollisionBase
                                            {
                                                Id = Guid.NewGuid(),
                                                CollidingObject1 = collidingObject1,
                                                CollidingObject2 = collidingObject2,
                                                Ignored = false,
                                                CollisionTypeId = collisionTypeData.id
                                            }
                                        );
                                    }
                                }
                                else
                                {
                                    objectBlock = block;
                                    targetBlock = lessonBlock;

                                    ruleCheckObject = true;
                                    ruleCheckTarget = true;

                                    foreach (Rule rule in objectRules)
                                    {
                                        ruleCheckObject = rule.CheckBlock(
                                            objectBlock,
                                            scanDate,
                                            collisionScanBlocks,
                                            scheduleSettings,
                                            collisionScanStaticData,
                                            logMaker
                                        );
                                        if (!ruleCheckObject)
                                        {
                                            break;
                                        }
                                    }

                                    foreach (Rule rule in targetRules)
                                    {
                                        ruleCheckTarget = rule.CheckBlock(
                                            targetBlock,
                                            scanDate,
                                            collisionScanBlocks,
                                            scheduleSettings,
                                            collisionScanStaticData,
                                            logMaker
                                        );
                                        if (!ruleCheckTarget)
                                        {
                                            break;
                                        }
                                    }

                                    if (ruleCheckObject && ruleCheckTarget)
                                    {
                                        bool ruleCheck = true;

                                        foreach (Rule rule in methodRules)
                                        {
                                            ruleCheck = rule.CheckPair(
                                                objectBlock,
                                                targetBlock,
                                                scanDate,
                                                collisionScanBlocks,
                                                scheduleSettings,
                                                collisionScanStaticData,
                                                logMaker
                                            );
                                            if (!ruleCheck)
                                            {
                                                break;
                                            }
                                        }

                                        if (ruleCheck)
                                        {
                                            string collidingObject1;
                                            string collidingObject2;

                                            if (objectBlock.Id.CompareTo(targetBlock.Id) < 0)
                                            {
                                                collidingObject1 = objectBlock.Id.ToString();
                                                collidingObject2 = targetBlock.Id.ToString();
                                            }
                                            else
                                            {
                                                collidingObject1 = targetBlock.Id.ToString();
                                                collidingObject2 = objectBlock.Id.ToString();
                                            }
                                            collisions.Add(
                                                new CollisionBase
                                                {
                                                    Id = Guid.NewGuid(),
                                                    CollidingObject1 = collidingObject1,
                                                    CollidingObject2 = collidingObject2,
                                                    Ignored = false,
                                                    CollisionTypeId = collisionTypeData.id
                                                }
                                            );
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
                case "lessonblock-week":
                    if (lessonBlock != null)
                    {
                        foreach (CollisionLessonBlock block in collisionScanBlocks.lessonBlocks)
                        {
                            if (lessonBlock.Id != block.Id)
                            {
                                CollisionLessonBlock objectBlock = lessonBlock;
                                CollisionLessonBlock targetBlock = block;
                                bool ruleCheckObject = true;
                                bool ruleCheckTarget = true;

                                foreach (Rule rule in objectRules)
                                {
                                    ruleCheckObject = rule.CheckBlock(
                                        objectBlock,
                                        scanDate,
                                        collisionScanBlocks,
                                        scheduleSettings,
                                        collisionScanStaticData,
                                        logMaker
                                    );
                                    if (!ruleCheckObject)
                                    {
                                        break;
                                    }
                                }

                                foreach (Rule rule in targetRules)
                                {
                                    ruleCheckTarget = rule.CheckBlock(
                                        targetBlock,
                                        scanDate,
                                        collisionScanBlocks,
                                        scheduleSettings,
                                        collisionScanStaticData,
                                        logMaker
                                    );
                                    if (!ruleCheckTarget)
                                    {
                                        break;
                                    }
                                }

                                if (ruleCheckObject && ruleCheckTarget)
                                {
                                    bool ruleCheck = true;

                                    foreach (Rule rule in methodRules)
                                    {
                                        ruleCheck = rule.CheckPair(
                                            objectBlock,
                                            targetBlock,
                                            scanDate,
                                            collisionScanBlocks,
                                            scheduleSettings,
                                            collisionScanStaticData,
                                            logMaker
                                        );
                                        if (!ruleCheck)
                                        {
                                            break;
                                        }
                                    }

                                    if (ruleCheck)
                                    {
                                        string collidingObject1;
                                        string collidingObject2;

                                        if (objectBlock.Id.CompareTo(targetBlock.Id) < 0)
                                        {
                                            collidingObject1 = objectBlock.Id.ToString();
                                            collidingObject2 = targetBlock.Id.ToString();
                                        }
                                        else
                                        {
                                            collidingObject1 = targetBlock.Id.ToString();
                                            collidingObject2 = objectBlock.Id.ToString();
                                        }
                                        collisions.Add(
                                            new CollisionBase
                                            {
                                                Id = Guid.NewGuid(),
                                                CollidingObject1 = collidingObject1,
                                                CollidingObject2 = collidingObject2,
                                                Ignored = false,
                                                CollisionTypeId = collisionTypeData.id
                                            }
                                        );
                                    }
                                }
                                else
                                {
                                    objectBlock = block;
                                    targetBlock = lessonBlock;

                                    ruleCheckObject = true;
                                    ruleCheckTarget = true;

                                    foreach (Rule rule in objectRules)
                                    {
                                        ruleCheckObject = rule.CheckBlock(
                                            objectBlock,
                                            scanDate,
                                            collisionScanBlocks,
                                            scheduleSettings,
                                            collisionScanStaticData,
                                            logMaker
                                        );
                                        if (!ruleCheckObject)
                                        {
                                            break;
                                        }
                                    }

                                    foreach (Rule rule in targetRules)
                                    {
                                        ruleCheckTarget = rule.CheckBlock(
                                            targetBlock,
                                            scanDate,
                                            collisionScanBlocks,
                                            scheduleSettings,
                                            collisionScanStaticData,
                                            logMaker
                                        );
                                        if (!ruleCheckTarget)
                                        {
                                            break;
                                        }
                                    }

                                    if (ruleCheckObject && ruleCheckTarget)
                                    {
                                        bool ruleCheck = true;

                                        foreach (Rule rule in methodRules)
                                        {
                                            ruleCheck = rule.CheckPair(
                                                objectBlock,
                                                targetBlock,
                                                scanDate,
                                                collisionScanBlocks,
                                                scheduleSettings,
                                                collisionScanStaticData,
                                                logMaker
                                            );
                                            if (!ruleCheck)
                                            {
                                                break;
                                            }
                                        }

                                        if (ruleCheck)
                                        {
                                            string collidingObject1;
                                            string collidingObject2;

                                            if (objectBlock.Id.CompareTo(targetBlock.Id) < 0)
                                            {
                                                collidingObject1 = objectBlock.Id.ToString();
                                                collidingObject2 = targetBlock.Id.ToString();
                                            }
                                            else
                                            {
                                                collidingObject1 = targetBlock.Id.ToString();
                                                collidingObject2 = objectBlock.Id.ToString();
                                            }
                                            collisions.Add(
                                                new CollisionBase
                                                {
                                                    Id = Guid.NewGuid(),
                                                    CollidingObject1 = collidingObject1,
                                                    CollidingObject2 = collidingObject2,
                                                    Ignored = false,
                                                    CollisionTypeId = collisionTypeData.id
                                                }
                                            );
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
                case "lessonblock-dayoff":
                    if (lessonBlock != null)
                    {
                        bool ruleCheck = true;
                        foreach (Rule rule in objectRules)
                        {
                            ruleCheck = rule.CheckBlock(
                                lessonBlock,
                                scanDate,
                                collisionScanBlocks,
                                scheduleSettings,
                                collisionScanStaticData,
                                logMaker
                            );
                            if (!ruleCheck)
                            {
                                break;
                            }
                        }

                        if (ruleCheck)
                        {
                            foreach (DayOff dayOff in collisionScanBlocks.daysOffFull)
                            {
                                ruleCheck = true;
                                foreach (Rule rule in targetRules)
                                {
                                    ruleCheck = rule.CheckDayOff(
                                        lessonBlock,
                                        dayOff,
                                        scanDate,
                                        collisionScanBlocks,
                                        scheduleSettings,
                                        collisionScanStaticData,
                                        logMaker
                                    );
                                    if (!ruleCheck)
                                    {
                                        break;
                                    }
                                }

                                if (ruleCheck)
                                {
                                    foreach (Rule rule in methodRules)
                                    {
                                        ruleCheck = rule.CheckDayOff(
                                            lessonBlock,
                                            dayOff,
                                            scanDate,
                                            collisionScanBlocks,
                                            scheduleSettings,
                                            collisionScanStaticData,
                                            logMaker
                                        );
                                        if (!ruleCheck)
                                        {
                                            break;
                                        }
                                    }

                                    if (ruleCheck)
                                    {
                                        collisions.Add(
                                            new CollisionBase
                                            {
                                                Id = Guid.NewGuid(),
                                                CollidingObject1 = lessonBlock.Id.ToString(),
                                                CollidingObject2 = dayOff.Id.ToString(),
                                                Ignored = false,
                                                CollisionTypeId = collisionTypeData.id
                                            }
                                        );
                                    }
                                }
                            }
                        }
                    }
                    break;
                case "lessonblock-availability":
                    if (lessonBlock != null)
                    {
                        bool ruleCheck = true;
                        foreach (Rule rule in objectRules)
                        {
                            ruleCheck = rule.CheckBlock(
                                lessonBlock,
                                scanDate,
                                collisionScanBlocks,
                                scheduleSettings,
                                collisionScanStaticData,
                                logMaker
                            );
                            if (!ruleCheck)
                            {
                                break;
                            }
                        }

                        if (ruleCheck)
                        {
                            foreach (
                                Availability availability in collisionScanStaticData.Availabilities
                            )
                            {
                                ruleCheck = true;
                                foreach (Rule rule in targetRules)
                                {
                                    ruleCheck = rule.CheckAvailability(
                                        lessonBlock,
                                        availability,
                                        scanDate,
                                        collisionScanBlocks,
                                        scheduleSettings,
                                        collisionScanStaticData,
                                        logMaker
                                    );
                                    if (!ruleCheck)
                                    {
                                        break;
                                    }
                                }

                                if (ruleCheck)
                                {
                                    foreach (Rule rule in methodRules)
                                    {
                                        ruleCheck = rule.CheckAvailability(
                                            lessonBlock,
                                            availability,
                                            scanDate,
                                            collisionScanBlocks,
                                            scheduleSettings,
                                            collisionScanStaticData,
                                            logMaker
                                        );
                                        if (!ruleCheck)
                                        {
                                            break;
                                        }
                                    }

                                    if (ruleCheck)
                                    {
                                        collisions.Add(
                                            new CollisionBase
                                            {
                                                Id = Guid.NewGuid(),
                                                CollidingObject1 = lessonBlock.Id.ToString(),
                                                CollidingObject2 = availability.Id.ToString(),
                                                Ignored = false,
                                                CollisionTypeId = collisionTypeData.id
                                            }
                                        );
                                    }
                                }
                            }
                        }
                    }
                    break;
                case "day":
                    {
                        List<CollisionLessonBlock> blocksToCheck = new List<CollisionLessonBlock>();

                        if (lessonBlock != null)
                        {
                            bool ruleCheck = true;
                            foreach (Rule rule in targetRules)
                            {
                                ruleCheck = rule.CheckBlock(
                                    lessonBlock,
                                    scanDate,
                                    collisionScanBlocks,
                                    scheduleSettings,
                                    collisionScanStaticData,
                                    logMaker
                                );
                                if (!ruleCheck)
                                {
                                    break;
                                }
                            }

                            if (ruleCheck)
                            {
                                blocksToCheck.Add(lessonBlock);
                            }
                        }

                        foreach (CollisionLessonBlock block in collisionScanBlocks.lessonBlocks)
                        {
                            bool ruleCheck = true;
                            if (block.LessonDate == scanDate)
                            {
                                foreach (Rule rule in targetRules)
                                {
                                    ruleCheck = rule.CheckBlock(
                                        block,
                                        scanDate,
                                        collisionScanBlocks,
                                        scheduleSettings,
                                        collisionScanStaticData,
                                        logMaker
                                    );
                                    if (!ruleCheck)
                                    {
                                        break;
                                    }
                                }

                                if (ruleCheck)
                                {
                                    blocksToCheck.Add(block);
                                }
                            }
                        }

                        List<Guid> blockIds = new List<Guid>();

                        foreach (Rule rule in methodRules)
                        {
                            List<Guid> blockIdsTemp = rule.CheckGroup(
                                blocksToCheck,
                                scanDate,
                                collisionScanBlocks,
                                scheduleSettings,
                                collisionScanStaticData,
                                logMaker
                            );

                            if (blockIds.Count == 0)
                            {
                                blockIds.AddRange(blockIdsTemp);
                            }
                            else
                            {
                                blockIds = blockIds.Intersect(blockIdsTemp).ToList();
                            }
                        }

                        foreach (Guid id in blockIds)
                        {
                            collisions.Add(
                                new CollisionBase
                                {
                                    Id = Guid.NewGuid(),
                                    CollidingObject1 = scanDate.ToString(),
                                    CollidingObject2 = id.ToString(),
                                    Ignored = false,
                                    CollisionTypeId = collisionTypeData.id
                                }
                            );
                        }
                    }
                    break;
                case "week":
                    {
                        List<CollisionLessonBlock> blocksToCheck = new List<CollisionLessonBlock>();

                        if (lessonBlock != null)
                        {
                            bool ruleCheck = true;
                            foreach (Rule rule in targetRules)
                            {
                                ruleCheck = rule.CheckBlock(
                                    lessonBlock,
                                    scanDate,
                                    collisionScanBlocks,
                                    scheduleSettings,
                                    collisionScanStaticData,
                                    logMaker
                                );
                                if (!ruleCheck)
                                {
                                    break;
                                }
                            }

                            if (ruleCheck)
                            {
                                blocksToCheck.Add(lessonBlock);
                            }
                        }

                        foreach (CollisionLessonBlock block in collisionScanBlocks.lessonBlocks)
                        {
                            bool ruleCheck = true;
                            foreach (Rule rule in targetRules)
                            {
                                ruleCheck = rule.CheckBlock(
                                    block,
                                    scanDate,
                                    collisionScanBlocks,
                                    scheduleSettings,
                                    collisionScanStaticData,
                                    logMaker
                                );
                                if (!ruleCheck)
                                {
                                    break;
                                }
                            }

                            if (ruleCheck)
                            {
                                blocksToCheck.Add(block);
                            }
                        }

                        List<Guid> blockIds = new List<Guid>();

                        foreach (Rule rule in methodRules)
                        {
                            List<Guid> blockIdsTemp = rule.CheckGroup(
                                blocksToCheck,
                                scanDate,
                                collisionScanBlocks,
                                scheduleSettings,
                                collisionScanStaticData,
                                logMaker
                            );

                            if (blockIds.Count == 0)
                            {
                                blockIds.AddRange(blockIdsTemp);
                            }
                            else
                            {
                                blockIds = blockIds.Intersect(blockIdsTemp).ToList();
                            }
                        }

                        foreach (Guid id in blockIds)
                        {
                            collisions.Add(
                                new CollisionBase
                                {
                                    Id = Guid.NewGuid(),
                                    CollidingObject1 = scanDate.ToString(),
                                    CollidingObject2 = id.ToString(),
                                    Ignored = false,
                                    CollisionTypeId = collisionTypeData.id
                                }
                            );
                        }
                    }
                    break;
                default:
                    break;
            }

            foreach (CollisionBase collision in collisions)
            {
                logMaker.writeLog(
                    (
                        collision.Id
                        + collision.CollidingObject1
                        + collision.CollidingObject2
                        + collision.CollisionTypeId
                    ).ToString()
                );
            }
            return collisions;
        }
    }
}
