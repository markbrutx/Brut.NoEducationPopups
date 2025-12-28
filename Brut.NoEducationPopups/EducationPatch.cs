// Brut.NoEducationPopups - Auto-complete child education popups
// by Brut | Open Source | MIT License
// https://github.com/markbrutx/Brut.NoEducationPopups

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Brut.NoEducationPopups
{
    public class AutoEducationBehavior : CampaignBehaviorBase
    {
        private static readonly Random _random = new Random();
        private List<string> _processedEducations = new List<string>();

        private static readonly int[] EducationAges = { 2, 5, 8, 11, 14, 16 };

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickHeroEvent.AddNonSerializedListener(this, OnDailyTickHero);
            Debug.Print("[Brut.NoEducationPopups] Events registered");
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("_processedEducations", ref _processedEducations);
        }

        private void OnDailyTickHero(Hero hero)
        {
            try
            {
                var settings = ModSettings.Instance;
                if (settings == null || !settings.IsEnabled)
                    return;

                if (hero == null || !hero.IsChild || hero.Clan != Clan.PlayerClan)
                    return;

                int age = (int)hero.Age;

                if (Array.IndexOf(EducationAges, age) < 0)
                    return;

                string educationKey = $"{hero.StringId}_{age}";
                if (_processedEducations.Contains(educationKey))
                    return;

                if (TryAutoCompleteEducation(hero, settings))
                {
                    _processedEducations.Add(educationKey);
                    Debug.Print($"[Brut.NoEducationPopups] Completed education for {hero.Name} at age {age}");
                }
            }
            catch (Exception ex)
            {
                Debug.Print($"[Brut.NoEducationPopups] Error in OnDailyTickHero: {ex.Message}");
            }
        }

        private bool TryAutoCompleteEducation(Hero child, ModSettings settings)
        {
            try
            {
                var educationBehavior = Campaign.Current?.GetCampaignBehavior<EducationCampaignBehavior>();
                if (educationBehavior == null)
                {
                    Debug.Print("[Brut.NoEducationPopups] EducationCampaignBehavior not found");
                    return false;
                }

                educationBehavior.GetStageProperties(child, out int pageCount);

                if (pageCount <= 0)
                {
                    Debug.Print($"[Brut.NoEducationPopups] No education pages available for {child.Name}");
                    return false;
                }

                Debug.Print($"[Brut.NoEducationPopups] {child.Name} has {pageCount} education pages");

                var chosenOptions = new List<string>();
                var allBonuses = new List<string>();

                for (int page = 0; page < pageCount; page++)
                {
                    educationBehavior.GetPageProperties(
                        child,
                        chosenOptions,
                        out TextObject title,
                        out TextObject description,
                        out TextObject instruction,
                        out EducationCampaignBehavior.EducationCharacterProperties[] defaultProps,
                        out string[] availableOptions
                    );

                    if (availableOptions == null || availableOptions.Length == 0)
                    {
                        Debug.Print($"[Brut.NoEducationPopups] No options on page {page + 1} for {child.Name}");
                        break;
                    }

                    string chosenOption = PickOption(educationBehavior, child, chosenOptions, availableOptions, settings);
                    chosenOptions.Add(chosenOption);

                    string details = FormatOptionDetails(educationBehavior, child, chosenOptions, chosenOption);
                    Debug.Print($"[Brut.NoEducationPopups] {child.Name} page {page + 1}: {details}");

                    // Collect bonuses for detailed notification
                    if (settings.DetailedNotifications)
                    {
                        var bonuses = GetOptionBonuses(educationBehavior, child, chosenOptions, chosenOption);
                        allBonuses.AddRange(bonuses);
                    }
                }

                if (chosenOptions.Count == 0)
                {
                    Debug.Print($"[Brut.NoEducationPopups] No choices made for {child.Name}");
                    return false;
                }

                educationBehavior.Finalize(child, chosenOptions);

                if (settings.ShowNotifications)
                {
                    TextObject message;
                    if (settings.DetailedNotifications && allBonuses.Count > 0)
                    {
                        message = new TextObject("{=brut_edu_complete_detailed}{CHILD_NAME}'s education completed. ({BONUSES})");
                        message.SetTextVariable("CHILD_NAME", child.Name);
                        message.SetTextVariable("BONUSES", string.Join(", ", allBonuses));
                    }
                    else
                    {
                        message = new TextObject("{=brut_edu_complete}{CHILD_NAME}'s education completed.");
                        message.SetTextVariable("CHILD_NAME", child.Name);
                    }
                    InformationManager.DisplayMessage(new InformationMessage(message.ToString(), Colors.Green));
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.Print($"[Brut.NoEducationPopups] Error in TryAutoCompleteEducation: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }

        private string PickOption(
            EducationCampaignBehavior educationBehavior,
            Hero child,
            List<string> previousChoices,
            string[] availableOptions,
            ModSettings settings)
        {
            if (!settings.IsAttributePriorityMode || !settings.HasPreferredAttributes)
            {
                return availableOptions[_random.Next(availableOptions.Length)];
            }

            var preferredAttributes = GetPreferredAttributes(settings);
            var scoredOptions = new List<(string option, int score)>();

            foreach (var optionKey in availableOptions)
            {
                int score = ScoreOption(educationBehavior, child, previousChoices, optionKey, preferredAttributes);
                scoredOptions.Add((optionKey, score));
            }

            int maxScore = scoredOptions.Max(x => x.score);
            var bestOptions = scoredOptions.Where(x => x.score == maxScore).Select(x => x.option).ToArray();

            return bestOptions[_random.Next(bestOptions.Length)];
        }

        private List<CharacterAttribute> GetPreferredAttributes(ModSettings settings)
        {
            var attributes = new List<CharacterAttribute>();

            if (settings.PreferVigor)
                attributes.Add(DefaultCharacterAttributes.Vigor);
            if (settings.PreferControl)
                attributes.Add(DefaultCharacterAttributes.Control);
            if (settings.PreferEndurance)
                attributes.Add(DefaultCharacterAttributes.Endurance);
            if (settings.PreferCunning)
                attributes.Add(DefaultCharacterAttributes.Cunning);
            if (settings.PreferSocial)
                attributes.Add(DefaultCharacterAttributes.Social);
            if (settings.PreferIntelligence)
                attributes.Add(DefaultCharacterAttributes.Intelligence);

            return attributes;
        }

        private string FormatOptionDetails(
            EducationCampaignBehavior behavior,
            Hero child,
            List<string> previousChoices,
            string optionKey)
        {
            try
            {
                behavior.GetOptionProperties(child, optionKey, previousChoices,
                    out TextObject title, out _, out _,
                    out (CharacterAttribute, int)[] attrs,
                    out (SkillObject, int)[] skills,
                    out (SkillObject, int)[] focus,
                    out _);

                var bonuses = new List<string>();

                if (attrs != null)
                    foreach (var (attr, val) in attrs)
                        if (attr != null && val != 0)
                            bonuses.Add($"+{val} {attr.Name}");

                if (skills != null)
                    foreach (var (skill, val) in skills)
                        if (skill != null && val != 0)
                            bonuses.Add($"+{val} {skill.Name}");

                if (focus != null)
                    foreach (var (skill, val) in focus)
                        if (skill != null && val != 0)
                            bonuses.Add($"+{val}F {skill.Name}");

                string bonusStr = bonuses.Count > 0 ? $" ({string.Join(", ", bonuses)})" : "";
                return $"\"{title}\"{bonusStr}";
            }
            catch
            {
                return $"'{optionKey}'";
            }
        }

        private List<string> GetOptionBonuses(
            EducationCampaignBehavior behavior,
            Hero child,
            List<string> previousChoices,
            string optionKey)
        {
            var bonuses = new List<string>();

            try
            {
                behavior.GetOptionProperties(child, optionKey, previousChoices,
                    out TextObject title, out _, out _,
                    out (CharacterAttribute, int)[] attrs,
                    out (SkillObject, int)[] skills,
                    out (SkillObject, int)[] focus,
                    out _);

                if (attrs != null)
                    foreach (var (attr, val) in attrs)
                        if (attr != null && val != 0)
                            bonuses.Add($"+{val} {attr.Name}");

                if (skills != null)
                    foreach (var (skill, val) in skills)
                        if (skill != null && val != 0)
                            bonuses.Add($"+{val} {skill.Name}");

                if (focus != null)
                    foreach (var (skill, val) in focus)
                        if (skill != null && val != 0)
                            bonuses.Add($"+{val}F {skill.Name}");
            }
            catch
            {
                // Ignore errors, return empty list
            }

            return bonuses;
        }

        private int ScoreOption(
            EducationCampaignBehavior educationBehavior,
            Hero child,
            List<string> previousChoices,
            string optionKey,
            List<CharacterAttribute> preferredAttributes)
        {
            try
            {
                educationBehavior.GetOptionProperties(
                    child,
                    optionKey,
                    previousChoices,
                    out TextObject optionTitle,
                    out TextObject description,
                    out TextObject effect,
                    out (CharacterAttribute, int)[] attributes,
                    out (SkillObject, int)[] skills,
                    out (SkillObject, int)[] focusPoints,
                    out EducationCampaignBehavior.EducationCharacterProperties[] educationCharacterProperties
                );

                int score = 0;

                if (attributes != null)
                {
                    foreach (var (attr, value) in attributes)
                    {
                        if (attr != null && preferredAttributes.Contains(attr))
                        {
                            score += value;
                        }
                    }
                }

                return score;
            }
            catch (Exception ex)
            {
                Debug.Print($"[Brut.NoEducationPopups] Error scoring option {optionKey}: {ex.Message}");
                return 0;
            }
        }
    }
}
