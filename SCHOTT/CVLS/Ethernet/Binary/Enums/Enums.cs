namespace SCHOTT.CVLS.Ethernet.Binary.Enums
{
    /// <summary>
    /// Command sets available to the BinarySocket
    /// </summary>
    public enum CommandSets
    {
        /// <summary>
        /// The System Command Set. This contains all general use functions.
        /// </summary>
        System,

        /// <summary>
        /// The Guest Command Set. This set contains all commands that can be accessed by a Guest login or higher.
        /// </summary>
        Guest,

        /// <summary>
        /// The Operator Command Set. This set contains all commands that can be accessed by a Operator login or higher.
        /// </summary>
        Operator,

        /// <summary>
        /// The Admin Command Set. This set contains all commands that can be accessed by a Admin login or higher.
        /// </summary>
        Admin
    }
    
    /// <summary>
    /// SystemCommands are base commands that require no login level
    /// </summary>
    public enum SystemCommands
    {
#pragma warning disable 1591
        SystemReportStatus,
        SystemReportControls,
        SystemReportBuffer,
        SystemReportFault,
        SystemReportLedTemp,
        SystemReportBoardTemp,
        SystemReportFanSpeed,
        SystemReportLightFeedBack,
        SystemReportEqualizerLightFeedBack,
        SystemReportEqualizerOutputPower,
        SystemReportEqualizerStatus,
        SystemReport5VMonitor,
        SystemReport24VMonitor,
        SystemReportFrontKnob,
        SystemReportFrontSwitch,
        SystemReportMultiportAnalogCh1,
        SystemReportMultiportAnalogCh2,
        SystemReportMultiportAnalogCh3,
        SystemReportMultiportAnalogCh4,
        SystemReportMultiportDigitalCh1,
        SystemReportMultiportDigitalCh2,
        SystemReportMultiportDigitalCh3,
        SystemReportMultiportDigitalCh4,

        SystemDisconnect = 50,
        SystemKeepalive,
        SystemMessageString,
        SystemReboot,
        SystemLastCommand,
        SystemOperationalMode,
        SystemDate,
        SystemModel,
        SystemSerial,
        SystemSettingsWriteCount,
        SystemFactorySettingsWriteCount,
        SystemSpiFlashEraseCount,
        SystemErrorCount,
        SystemLoginRequest,
        SystemLoginSuccessful,
        SystemLoginFailed,
        SystemLogout,
        SystemLoginUsername,
        SystemLoginPassword,
        SystemFirmwareVersion
#pragma warning restore 1591
    }

    /// <summary>
    /// GuestCommands require that a guest login be used. Currently the CVLS does not make use of this command level.
    /// </summary>
    public enum GuestCommands
    {
#pragma warning disable 1591
        None
#pragma warning restore 1591
    }

    /// <summary>
    /// OperatorCommands require that a user level login be used.
    /// </summary>
    public enum OperatorCommands
    {
#pragma warning disable 1591
        OperatorDemoEnabled,
        OperatorLedOutputEnabled,
        OperatorLedGangedTriggerEnabled,
        OperatorLedKnobMode,
        OperatorLedSingleChannelEnabled,
        OperatorLedOutputPower,
        OperatorLedChannelEnabled,
        OperatorLedChannelShutdownActiveHigh,
        OperatorLedChannelPower,

        OperatorContinuousStrobeEnabled = 20,
        OperatorContinuousStrobeFrequency,
        OperatorContinuousStrobeSingleChannelEnabled,
        OperatorContinuousStrobeChannelActiveHigh,
        OperatorContinuousStrobeChannelPhaseShift,
        OperatorContinuousStrobeChannelDutyCycle,

        OperatorTriggeredStrobeEnabled = 40,
        OperatorTriggeredStrobeSingleChannelEnabled,
        OperatorTriggeredStrobeGangedTriggerEnabled,
        OperatorTriggeredStrobeChannelFallingEdgeTrigger,
        OperatorTriggeredStrobeChannelOnTime,
        OperatorTriggeredStrobeChannelDelay,

        OperatorEqualizerEnabled = 60,
        OperatorEqualizerDelay,
        OperatorEqualizerTarget,
        OperatorEqualizerOutput,
        OperatorEqualizerLightFeedback,

        OperatorFanManualOverride = 80,
        OperatorFanMaxSpeed,
        OperatorFanTargetTemp
#pragma warning restore 1591
    }

    /// <summary>
    /// AdminCommands require that a admin level login be used.
    /// </summary>
    public enum AdminCommands
    {
#pragma warning disable 1591
        AdminSaveSettings,
        AdminRestoreSettings,
        AdminRestoreFactorySettings,
        AdminRestoreFactorySettingsPreserveNetwork,
        AdminFirmware,
        AdminFirmwareLoad,
        AdminConfigExport,
        AdminConfigImport,
        AdminConfigImportComplete,
        AdminConfigImportLogRead,
        AdminConfigImportCancel,
        AdminSettingsGet,
        AdminConfigExportCount,

        AdminSettingsLoginTimeoutEnable = 20,
        AdminSettingsLoginTimeoutMinutes,
        AdminSettingsLoginRequireAdmin,
        AdminSettingsLoginRequireUser,
        AdminSettingsLoginAllowRememberPassword,

        AdminSettingsLockoutFrontControlsEnabled = 40,
        AdminSettingsLockoutMultiportControlsEnabled,

        AdminSettingsNetworkHostname = 60,
        AdminSettingsNetworkDhcpEnabled,
        AdminSettingsNetworkIpAddress,
        AdminSettingsNetworkMask,
        AdminSettingsNetworkGateway,
        AdminSettingsPrimaryDns,
        AdminSettingsSecondaryDns,
        AdminSettingsNetworkRestart,

        AdminSettingsEmailServername = 80,
        AdminSettingsEmailUsername,
        AdminSettingsEmailPassword,
        AdminSettingsEmailFrom,
        AdminSettingsEmailPort,
        AdminSettingsEmailSslEnabled,
        AdminSettingsEmailAuthenticationEnabled,

        AdminSocketLegacyPort = 100,
        AdminSocketLegacyEnabled,

        AdminSocketBinaryPort = 120,
        AdminSocketBinaryEnabled,

        AdminUartBaudRateIndex = 140,
        AdminUartStopBits,
        AdminUartParity,
        AdminUartRestart,

        AdminLogsClear = 160,
        AdminLogsRead,
        AdminLogsCount,

        AdminSocketConnectionLegacyIp = 180,
        AdminSocketConnectionLegacyKick,
        AdminSocketConnectionBinaryIp,
        AdminSocketConnectionBinaryKick,

        AdminUsersCount = 200,
        AdminUsersMax,
        AdminUsersNew,
        AdminUsersEdit,
        AdminUsersSave,
        AdminUsersDelete,
        AdminUsersUsername,
        AdminUsersPassword,
        AdminUsersSecurityLevel,
        AdminUsersPhone,
        AdminUsersEmail,

        AdminNtpCount = 250,
        AdminNtpMax,
        AdminNtpNew,
        AdminNtpEdit,
        AdminNtpSave,
        AdminNtpDelete,
        AdminNtpServerEnabled,
        AdminNtpName,
        AdminNtpPort,
        AdminNtpRank,
        AdminNtpMinutesBetweenPolls,
        AdminNtpTimeout,
        AdminNtpMaxAttempts
#pragma warning restore 1591
    }
}