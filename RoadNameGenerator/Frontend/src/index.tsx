import { useEffect, useRef, useState } from "react";
import { createPortal } from "react-dom";
import { ModRegistrar } from "cs2/modding";
import { bindValue, trigger, useValue } from "cs2/api";
import { useLocalization } from "cs2/l10n";
import { Panel, Tooltip } from "cs2/ui";
import styles from "./road-names.module.scss";

const group = "roadNameGenerator";
const showButton$ = bindValue<boolean>(group, "showButton", true);
const open$ = bindValue<boolean>(group, "open", false);
const hasRoad$ = bindValue<boolean>(group, "hasRoad", false);
const roadName$ = bindValue<string>(group, "roadName", "");
const token$ = bindValue<string>(group, "token", "");
const message$ = bindValue<string>(group, "message", "");
const customCount$ = bindValue<number>(group, "customCount", 0);
const listError$ = bindValue<boolean>(group, "listError", false);
const exclusionsEnabled$ = bindValue<boolean>(group, "exclusionsEnabled", true);
const excludedCount$ = bindValue<number>(group, "excludedCount", 0);
const exclusionError$ = bindValue<boolean>(group, "exclusionError", false);
const namingStyle$ = bindValue<number>(group, "namingStyle", 0);
const customMode$ = bindValue<number>(group, "customMode", 0);
const autoNaming$ = bindValue<boolean>(group, "autoNaming", true);
const existingMode$ = bindValue<number>(group, "existingMode", 0);
const overwriteNames$ = bindValue<boolean>(group, "overwriteNames", false);
const detailedLogging$ = bindValue<boolean>(group, "detailedLogging", false);
// Explicit values match the existing saved C# enum; keep Swiss variants together.
const namingStyles = [
    { value: 0, key: "American" }, { value: 1, key: "Australian" },
    { value: 2, key: "British" }, { value: 3, key: "Canadian" },
    { value: 4, key: "Danish" }, { value: 5, key: "Dutch" },
    { value: 6, key: "French" }, { value: 7, key: "German" },
    { value: 8, key: "Italian" }, { value: 9, key: "Mexican" },
    { value: 10, key: "Norwegian" }, { value: 11, key: "Polish" },
    { value: 19, key: "PortugueseBrazil" },
    { value: 18, key: "PortuguesePortugal" },
    { value: 12, key: "Spanish" }, { value: 13, key: "Swedish" },
    { value: 15, key: "Swiss" }, { value: 14, key: "SwissFrench" },
    { value: 16, key: "SwissItalian" }, { value: 17, key: "SwissRomansh" },
];
const suggestion0$ = bindValue<string>(group, "suggestion0", "");
const suggestion1$ = bindValue<string>(group, "suggestion1", "");
const suggestion2$ = bindValue<string>(group, "suggestion2", "");
const suggestion3$ = bindValue<string>(group, "suggestion3", "");
const suggestion4$ = bindValue<string>(group, "suggestion4", "");

function useText() {
    const { translate } = useLocalization();
    return (key: string) => translate(`RoadNameGenerator.UI.${key}`) ?? key;
}

function Launcher() {
    const show = useValue(showButton$);
    const t = useText();
    if (!show) return null;
    return <Tooltip tooltip={t("Toggle")}>
        <button className={styles.launcher}
            aria-label={t("Toggle")} onClick={() => trigger(group, "toggle")}>
            <svg viewBox="0 0 32 32" className={styles.icon}>
                <path d="M15 4h3v25h-3zM3 6h22l5 5-5 5H3z" fill="currentColor" />
                <path d="M8 10h13v2H8z" fill="#0c1e2c" />
                <path d="M7 19h18v6H7l-4-3z" fill="currentColor" />
            </svg>
        </button>
    </Tooltip>;
}

function NameSourceDropdown({ label, hint, items, selected, onSelect }: {
    label: string; hint: string; items: { value: number; label: string }[];
    selected: number; onSelect: (value: number) => void;
}) {
    const [open, setOpen] = useState(false);
    const [offset, setOffset] = useState(0);
    const [position, setPosition] = useState({ left: 0, top: 0 });
    const anchor = useRef<HTMLButtonElement>(null);
    const track = useRef<HTMLDivElement>(null);
    const dragging = useRef(false);
    const visible = 8;
    const maximum = Math.max(0, items.length - visible);
    const scrollTo = (value: number) => setOffset(Math.max(0, Math.min(maximum, value)));
    useEffect(() => {
        const move = (event: MouseEvent) => {
            if (!dragging.current || !track.current) return;
            const rect = track.current.getBoundingClientRect();
            if (rect.height > 0) scrollTo(Math.round((event.clientY - rect.top) / rect.height * maximum));
        };
        const stop = () => { dragging.current = false; };
        const escape = (event: KeyboardEvent) => {
            if (event.key === "Escape") { setOpen(false); stop(); }
        };
        window.addEventListener("mousemove", move);
        window.addEventListener("mouseup", stop);
        window.addEventListener("keydown", escape);
        return () => {
            window.removeEventListener("mousemove", move);
            window.removeEventListener("mouseup", stop);
            window.removeEventListener("keydown", escape);
        };
    }, [maximum]);
    const toggle = () => {
        if (!anchor.current) return;
        const rect = anchor.current.getBoundingClientRect();
        const scale = rect.width / 210 || 1;
        const height = 266 * scale;
        const top = rect.bottom + height + 4 * scale <= window.innerHeight
            ? rect.bottom + 4 * scale : Math.max(0, rect.top - height - 4 * scale);
        setPosition({ left: Math.max(0, rect.right - 260 * scale), top });
        scrollTo(Math.max(0, items.findIndex(item => item.value === selected) - 3));
        setOpen(!open);
    };
    return <>
        <Tooltip tooltip={hint}>
            <button ref={anchor} type="button" className={styles.styleToggle}
                aria-expanded={open} onMouseDown={event => event.stopPropagation()}
                onClick={toggle}>
                <span className={styles.styleValue}>{label}</span>
                <svg className={styles.styleIndicator} viewBox="0 0 16 16">
                    <path d={open ? "M3 10l5-5 5 5" : "M3 6l5 5 5-5"}
                        fill="none" stroke="currentColor" strokeWidth="2" />
                </svg>
            </button>
        </Tooltip>
        {open && createPortal(<div className={styles.styleLayer}
            onMouseDown={event => { event.stopPropagation(); setOpen(false); dragging.current = false; }}>
            <div className={styles.styleMenu} style={{ left: position.left, top: position.top }}
                onMouseDown={event => event.stopPropagation()}
                onPointerDown={event => event.stopPropagation()}
                onClick={event => event.stopPropagation()}
                onWheel={event => {
                    event.preventDefault(); event.stopPropagation();
                    if (event.deltaY) setOffset(value => Math.max(0, Math.min(maximum,
                        value + (event.deltaY > 0 ? 1 : -1))));
                }}>
                <div className={styles.styleOptions} role="listbox">
                    {items.slice(offset, offset + visible).map(item =>
                        <button key={item.value} type="button" role="option"
                            aria-selected={selected === item.value}
                            className={`${styles.styleItem} ${selected === item.value ? styles.styleSelected : ""}`}
                            onClick={event => { event.stopPropagation(); onSelect(item.value); setOpen(false); }}>
                            {item.label}
                        </button>)}
                </div>
                <div ref={track} className={styles.styleTrack} role="scrollbar" tabIndex={0}
                    aria-orientation="vertical" aria-valuemin={0} aria-valuemax={maximum} aria-valuenow={offset}
                    onMouseDown={event => {
                        if (event.button !== 0) return;
                        event.preventDefault(); event.stopPropagation(); dragging.current = true;
                        const rect = event.currentTarget.getBoundingClientRect();
                        if (rect.height > 0) scrollTo(Math.round((event.clientY - rect.top) / rect.height * maximum));
                    }}
                    onKeyDown={event => {
                        const values: Record<string, number> = { ArrowDown: offset + 1,
                            ArrowUp: offset - 1, PageDown: offset + visible, PageUp: offset - visible,
                            Home: 0, End: maximum };
                        if (event.key in values) {
                            event.preventDefault(); event.stopPropagation(); scrollTo(values[event.key]);
                        }
                    }}>
                    <div className={styles.styleThumb} style={{ height: `${visible / items.length * 100}%`,
                        top: `${offset / items.length * 100}%` }} />
                </div>
            </div>
        </div>, document.body)}
    </>;
}

function RoadNamesPanel() {
    const show = useValue(showButton$);
    const open = useValue(open$);
    const hasRoad = useValue(hasRoad$);
    const roadName = useValue(roadName$);
    const token = useValue(token$);
    const message = useValue(message$);
    const count = useValue(customCount$);
    const listError = useValue(listError$);
    const namingStyle = useValue(namingStyle$);
    const excludedCount = useValue(excludedCount$);
    const exclusionsEnabled = useValue(exclusionsEnabled$);
    const exclusionError = useValue(exclusionError$);
    const customMode = useValue(customMode$);
    const autoNaming = useValue(autoNaming$);
    const existingMode = useValue(existingMode$);
    const overwriteNames = useValue(overwriteNames$);
    const detailedLogging = useValue(detailedLogging$);
    const [settingsOpen, setSettingsOpen] = useState(false);
    const [confirmScan, setConfirmScan] = useState(false);
    const suggestions = [useValue(suggestion0$), useValue(suggestion1$),
        useValue(suggestion2$), useValue(suggestion3$), useValue(suggestion4$)].filter(Boolean);
    const { translate } = useLocalization();
    const t = useText();
    const styleName = (key: string) => translate(
        `Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[${key}]`) ?? key;
    const selectedStyle = namingStyles.find(style => style.value === namingStyle) ?? namingStyles[0];
    const sourceName = customMode === 1 ? t("CustomSource") : customMode === 2
        ? t("MixedSource") : styleName(selectedStyle.key);
    const sourceItems = [{ value: -1, label: t("CustomSource") },
        ...namingStyles.map(style => ({ value: style.value, label: styleName(style.key) }))];
    const scanModes = [t("ScanGameOnly"), t("ScanNonMatching"), t("ScanAll")];
    useEffect(() => {
        if (!open) { setSettingsOpen(false); setConfirmScan(false); }
    }, [open]);
    if (!show || !open) return null;

    return <>
        <Panel className={styles.panel} draggable initialPosition={{ x: 0.65, y: 0.2 }}
            header={t("Title")} onClose={() => {
                setSettingsOpen(false);
                setConfirmScan(false);
                trigger(group, "close");
            }}>
            <div className={styles.content}>
                <div className={styles.styleRow}>
                    <span className={styles.label}>{t("NamingStyle")}</span>
                    <NameSourceDropdown label={sourceName} hint={t("NamingStyleHint")}
                        items={sourceItems} selected={customMode === 1 ? -1 : customMode === 0 ? namingStyle : -2}
                        onSelect={value => value === -1 ? trigger(group, "setCustomMode", 1)
                            : trigger(group, "setNamingStyle", value)} />
                    <Tooltip tooltip={t("SettingsTitle")}>
                        <button className={styles.gearButton} type="button"
                            aria-label={t("SettingsTitle")} aria-expanded={settingsOpen}
                            onClick={() => { setSettingsOpen(value => !value); setConfirmScan(false); }}>
                            <svg viewBox="0 0 24 24" width="20rem" height="20rem"
                                fill="none" stroke="currentColor" strokeWidth="1.8" strokeLinecap="round" strokeLinejoin="round">
                                <path d="M12 3l1.7 2.1 2.6-.4.9 2.5 2.6.8-.5 2.6 1.6 2-1.6 2 .5 2.6-2.6.8-.9 2.5-2.6-.4L12 22l-1.7-2.1-2.6.4-.9-2.5-2.6-.8.5-2.6L3 12l1.7-2-.5-2.6 2.6-.8.9-2.5 2.6.4L12 3z"/>
                                <circle cx="12" cy="12" r="3.1" />
                            </svg>
                        </button>
                    </Tooltip>
                </div>
                {hasRoad ? <>
                    <div className={styles.compactLabel}>{t("CurrentName")}</div>
                    <div className={styles.currentName}>{roadName}</div>
                    <div className={styles.compactLabel}>{t("Suggestions")}</div>
                    {suggestions.length > 0 ? suggestions.map(name =>
                        <Tooltip key={`${token}:${name}`} tooltip={t("ApplyHint")}>
                            <button className={styles.suggestion} onClick={() => trigger(group, "apply", token, name)}>
                                <span>{name}</span><span className={styles.apply}>{t("Apply")}</span>
                            </button>
                        </Tooltip>) : <div className={styles.hint}>{t(excludedCount > 0 ? "ExclusionsUnavailable" : "NoSuggestions")}</div>}
                    <button className={styles.button + " " + styles.fullButton}
                        onClick={() => trigger(group, "refresh")}>{t("Refresh")}</button>
                </> : <div className={styles.hint}>{t("SelectRoad")}</div>}
                {message && <div className={styles.message}>{translate(message) ?? message}</div>}
                <div className={styles.scanSection}>
                    <button className={styles.scanButton} onClick={() => setConfirmScan(true)}>
                        {existingMode === 2 ? t("RenameAll") : t("CheckExisting")}
                    </button>
                    {confirmScan && <div className={styles.confirmBox}>
                        <div className={styles.hint}>{t("ConfirmScan")} {scanModes[existingMode] ?? ""}</div>
                        <div className={styles.buttons}>
                            <button className={styles.scanButton} onClick={() => {
                                trigger(group, "scanExisting");
                                setConfirmScan(false);
                            }}>{t("Confirm")}</button>
                            <button className={styles.button} onClick={() => setConfirmScan(false)}>{t("Cancel")}</button>
                        </div>
                    </div>}
                </div>
            </div>
        </Panel>
        {settingsOpen && <Panel className={styles.settingsPanel} draggable
            initialPosition={{ x: 0.42, y: 0.2 }}
            header={t("SettingsTitle")} onClose={() => setSettingsOpen(false)}>
            <div className={styles.settingsContent}>
                <div className={styles.sectionTitle}>{t("AutomaticNamingSection")}</div>
                <label className={styles.checkRow}>
                    <input type="checkbox" checked={autoNaming}
                        onChange={event => trigger(group, "setAutoNaming", event.currentTarget.checked ? 1 : 0)} />
                    <span>{t("AutomaticNaming")}</span>
                </label>
                <div className={styles.settingsSection}>
                    <div className={styles.sectionTitle}>{t("ExistingRoadsSection")}</div>
                    <div className={styles.compactLabel}>{t("ScanMode")}</div>
                    <select className={styles.select} value={existingMode}
                        onChange={event => { trigger(group, "setExistingMode", Number(event.currentTarget.value)); setConfirmScan(false); }}>
                        {scanModes.map((name, index) => <option key={index} value={index}>{name}</option>)}
                    </select>
                    <label className={styles.checkRow}>
                        <input type="checkbox" checked={overwriteNames}
                            onChange={event => trigger(group, "setOverwriteNames", event.currentTarget.checked ? 1 : 0)} />
                        <span>{t("OverwriteNames")}</span>
                    </label>
                    <div className={styles.hint}>{t("ScanExplanation")}</div>
                </div>
                <div className={styles.settingsSection}>
                    <div className={styles.sectionTitle}>{t("CustomListsSection")}</div>
                    <div className={styles.compactLabel}>{count > 0 ? `${t("CustomList")} (${count})` : t("NoList")}</div>
                    {listError && <div className={styles.error}>{t("ListError")}</div>}
                    <div className={styles.buttons}>
                        <button className={styles.button} onClick={() => trigger(group, "openFolder")}>{t("OpenFolder")}</button>
                        <button className={styles.button} onClick={() => trigger(group, "reloadList")}>{t("ReloadList")}</button>
                    </div>
                    <label className={styles.checkRow}>
                        <input type="checkbox" checked={exclusionsEnabled}
                            onChange={event => trigger(group, "setExclusions", event.currentTarget.checked ? 1 : 0)} />
                        <span>{t("UseExclusions")}</span>
                    </label>
                    <div className={styles.compactLabel}>{excludedCount > 0
                        ? `${t("ExclusionList")} (${excludedCount})` : t("NoExclusions")}</div>
                    {exclusionError && <div className={styles.error}>{t("ListError")}</div>}
                    <div className={styles.buttons}>
                        <button className={styles.button} onClick={() => trigger(group, "reloadExclusions")}>{t("ReloadExclusions")}</button>
                        <button className={styles.button} disabled={!exclusionsEnabled || excludedCount === 0}
                            onClick={() => trigger(group, "removeExclusions")}>{t("RemoveExclusions")}</button>
                    </div>
                </div>
                <div className={styles.settingsSection}>
                    <label className={styles.checkRow}>
                        <input type="checkbox" checked={detailedLogging}
                            onChange={event => trigger(group, "setDetailedLogging", event.currentTarget.checked ? 1 : 0)} />
                        <span>{t("DetailedLogging")}</span>
                    </label>
                </div>
            </div>
        </Panel>}
    </>;
}

const register: ModRegistrar = registry => {
    registry.append("GameTopRight", Launcher);
    registry.append("Game", RoadNamesPanel);
};
export default register;
