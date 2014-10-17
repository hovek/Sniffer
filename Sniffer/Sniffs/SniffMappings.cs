using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sniffer.Sniffs
{
    public class SniffMappings
    {
        public static Dictionary<SniffEnum, SniffMappings> SniffsMappings = new Dictionary<SniffEnum, SniffMappings>();

        public Type Sniffer;
        public Type SnifferSettings;
        public Type SnifferSettingsForm;
        public Type UserSettings;
        public Type UserSettingsForm;

        static SniffMappings()
        {
            SniffsMappings.Add(
                SniffEnum.Dvoznak,
                new SniffMappings()
                {
                    Sniffer = typeof(Dvoznak.Dvoznak),
                    SnifferSettings = typeof(Settings.SniffSettings),
                    SnifferSettingsForm = typeof(Settings.Forms.SniffSettingsForm),
                    UserSettings = typeof(Dvoznak.DvoznakUserSettings),
                    UserSettingsForm = typeof(Dvoznak.DvoznakUserSettingsForm)
                }
            );

            SniffsMappings.Add(
                SniffEnum.Deki,
                new SniffMappings()
                {
                    Sniffer = typeof(Deki.Deki),
                    SnifferSettings = typeof(Settings.SniffSettings),
                    SnifferSettingsForm = typeof(Settings.Forms.SniffSettingsForm),
                    UserSettings = typeof(Deki.DekiUserSettings),
                    UserSettingsForm = typeof(Deki.DekiUserSettingsForm),
                }
            );

            SniffsMappings.Add(
                SniffEnum.Veky,
                new SniffMappings()
                {
                    Sniffer = typeof(Veky.Veky),
                    SnifferSettings = typeof(Settings.SniffSettings),
                    SnifferSettingsForm = typeof(Settings.Forms.SniffSettingsForm),
                    UserSettings = typeof(Veky.VekyUserSettings),
                    UserSettingsForm = typeof(Veky.VekyUserSettingsForm),
                }
            );

            SniffsMappings.Add(
                SniffEnum.Njuskalo,
                new SniffMappings()
                {
                    Sniffer = typeof(Njuskalo.Njuskalo),
                    SnifferSettings = typeof(Settings.SniffSettings),
                    SnifferSettingsForm = typeof(Settings.Forms.SniffSettingsForm),
                    UserSettings = typeof(Njuskalo.NjuskaloUserSettings),
                    UserSettingsForm = typeof(Njuskalo.NjuskaloUserSettingsForm),
                }
            );
        }
    }
}
