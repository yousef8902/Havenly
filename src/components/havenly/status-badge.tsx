import { Check, Clock, X, Ban, CircleCheck, FileText, TriangleAlert } from "lucide-react";
import type { ComponentType } from "react";

type Tone = { className: string; Icon: ComponentType<{ className?: string }>; label: string };

const map: Record<string, Tone> = {
  pending: { className: "bg-warning/15 text-warning-foreground border-warning/40", Icon: Clock, label: "Pending" },
  approved: { className: "bg-success/12 text-success border-success/35", Icon: Check, label: "Approved" },
  completed: { className: "bg-primary/10 text-primary border-primary/30", Icon: CircleCheck, label: "Completed" },
  cancelled: { className: "bg-muted text-muted-foreground border-border", Icon: Ban, label: "Cancelled" },
  rejected: { className: "bg-destructive/10 text-destructive border-destructive/30", Icon: X, label: "Rejected" },
  draft: { className: "bg-muted text-muted-foreground border-border", Icon: FileText, label: "Draft" },
  active: { className: "bg-success/12 text-success border-success/35", Icon: Check, label: "Active" },
  suspended: { className: "bg-destructive/10 text-destructive border-destructive/30", Icon: TriangleAlert, label: "Suspended" },
};

export function StatusBadge({ status, className = "" }: { status: string; className?: string }) {
  const key = status.toLowerCase();
  const tone: Tone = map[key] ?? {
    className: "bg-muted text-muted-foreground border-border",
    Icon: Clock,
    label: status,
  };
  const Icon = tone.Icon;
  return (
    <span
      className={`inline-flex items-center gap-1.5 rounded-full border px-2.5 py-1 text-xs font-semibold ${tone.className} ${className}`}
    >
      <Icon className="size-3.5" />
      {tone.label}
    </span>
  );
}