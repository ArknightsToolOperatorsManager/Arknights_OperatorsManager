using Arknights_OperatorsManager.Domain.Enums;
using Arknights_OperatorsManager.Domain.ValueObjects;
using FluentValidation;
using llyrtkframework.Validation;

namespace Arknights_OperatorsManager.Domain.Validators;

/// <summary>
/// オペレーター育成状態のバリデーター
/// </summary>
public class OperatorStateValidator : AbstractValidatorBase<OperatorStateValidationContext>
{
    public OperatorStateValidator()
    {
        // 1. 昇進段階がレアリティの上限以内か
        RuleFor(ctx => ctx.State.Promotion.Value)
            .LessThanOrEqualTo(ctx => ctx.Rarity.MaxPromotion)
            .WithMessage(ctx => $"Promotion {ctx.State.Promotion.Value} exceeds maximum {ctx.Rarity.MaxPromotion} for rarity {ctx.Rarity.Value}");

        // 2. レベルが昇進段階の上限以内か（昇進段階が有効な場合のみ）
        RuleFor(ctx => ctx.State.Level)
            .Must((ctx, level) =>
            {
                // 昇進段階が無効な場合はスキップ（別のルールで検出される）
                if (ctx.State.Promotion.Value > ctx.Rarity.MaxPromotion)
                    return true;

                return level <= ctx.Rarity.GetMaxLevel(ctx.State.Promotion);
            })
            .WithMessage(ctx =>
            {
                if (ctx.State.Promotion.Value <= ctx.Rarity.MaxPromotion)
                {
                    var maxLevel = ctx.Rarity.GetMaxLevel(ctx.State.Promotion);
                    return $"Level {ctx.State.Level} exceeds maximum {maxLevel} for rarity {ctx.Rarity.Value} at promotion {ctx.State.Promotion.Value}";
                }
                return $"Level validation skipped due to invalid promotion";
            });

        // レベルは最低1
        RuleFor(ctx => ctx.State.Level)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Level must be at least 1");

        // 3. スキルレベルが昇進段階の上限以内か
        RuleFor(ctx => ctx.State.SkillLevel)
            .LessThanOrEqualTo(ctx => ctx.State.Promotion.MaxSkillLevel)
            .WithMessage(ctx => $"Skill level {ctx.State.SkillLevel} exceeds maximum {ctx.State.Promotion.MaxSkillLevel} for promotion {ctx.State.Promotion.Value}");

        // スキルレベルは最低1
        RuleFor(ctx => ctx.State.SkillLevel)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(7)
            .WithMessage("Skill level must be between 1 and 7");

        // 4. スキル特化は昇進2以上でのみ設定可能か
        RuleFor(ctx => ctx)
            .Must(ctx => ctx.State.Promotion.CanMastery ||
                         (ctx.State.Skill1Mastery.Value == 0 &&
                          ctx.State.Skill2Mastery.Value == 0 &&
                          ctx.State.Skill3Mastery.Value == 0))
            .WithMessage("Skill mastery can only be set at Elite 2");

        // 5. オペレーターのスキル数以上のスキルに特化が設定されていないか
        RuleFor(ctx => ctx)
            .Must(ctx => ctx.SkillCount >= 2 || ctx.State.Skill2Mastery.Value == 0)
            .WithMessage("Operator only has 1 skill, Skill2Mastery must be 0");

        RuleFor(ctx => ctx)
            .Must(ctx => ctx.SkillCount >= 3 || ctx.State.Skill3Mastery.Value == 0)
            .WithMessage("Operator only has 2 skills, Skill3Mastery must be 0");

        // 6. モジュールは昇進2以上でのみ設定可能か
        RuleFor(ctx => ctx)
            .Must(ctx => ctx.State.Promotion.CanModule ||
                         (ctx.State.ModuleX.Value == 0 &&
                          ctx.State.ModuleY.Value == 0 &&
                          ctx.State.ModuleD.Value == 0 &&
                          ctx.State.ModuleA.Value == 0))
            .WithMessage("Modules can only be set at Elite 2");

        // 7. オペレーターが持たないモジュールに設定されていないか
        RuleFor(ctx => ctx)
            .Must(ctx => ctx.AvailableModules.Contains(ModuleType.X) || ctx.State.ModuleX.Value == 0)
            .WithMessage("Operator does not have Module X");

        RuleFor(ctx => ctx)
            .Must(ctx => ctx.AvailableModules.Contains(ModuleType.Y) || ctx.State.ModuleY.Value == 0)
            .WithMessage("Operator does not have Module Y");

        RuleFor(ctx => ctx)
            .Must(ctx => ctx.AvailableModules.Contains(ModuleType.D) || ctx.State.ModuleD.Value == 0)
            .WithMessage("Operator does not have Module D");

        RuleFor(ctx => ctx)
            .Must(ctx => ctx.AvailableModules.Contains(ModuleType.A) || ctx.State.ModuleA.Value == 0)
            .WithMessage("Operator does not have Module A");

        // 8. モジュールレベル条件（昇進2かつ一定レベル以上）
        RuleFor(ctx => ctx)
            .Must(ctx =>
            {
                if (!ctx.State.Promotion.CanModule)
                    return true; // 昇進2未満なら既に上でチェック済み

                var moduleRequirement = ctx.Rarity.Value switch
                {
                    6 => 60,
                    5 => 50,
                    4 => 40,
                    _ => int.MaxValue // ★1-3はモジュール非対応
                };

                var hasAnyModule = ctx.State.ModuleX.Value > 0 ||
                                   ctx.State.ModuleY.Value > 0 ||
                                   ctx.State.ModuleD.Value > 0 ||
                                   ctx.State.ModuleA.Value > 0;

                return !hasAnyModule || ctx.State.Level >= moduleRequirement;
            })
            .WithMessage(ctx => $"Modules require Elite 2 level {ctx.Rarity.Value switch { 6 => 60, 5 => 50, 4 => 40, _ => 0 }} or higher for rarity {ctx.Rarity.Value}");
    }
}
