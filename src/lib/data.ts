import p1 from "@/assets/p1.jpg";
import p2 from "@/assets/p2.jpg";
import p3 from "@/assets/p3.jpg";
import p4 from "@/assets/p4.jpg";
import p5 from "@/assets/p5.jpg";
import p6 from "@/assets/p6.jpg";
import p7 from "@/assets/p7.jpg";
import hero from "@/assets/hero.jpg";

export const images = { p1, p2, p3, p4, p5, p6, p7, hero };

export type ListingStatus = "approved" | "pending" | "rejected" | "draft";
export type BookingStatus =
  | "pending"
  | "approved"
  | "completed"
  | "cancelled"
  | "rejected";

export type Property = {
  id: string;
  title: string;
  city: string;
  country: string;
  neighbourhood: string;
  price: number;
  rating: number;
  reviews: number;
  guests: number;
  bedrooms: number;
  beds: number;
  baths: number;
  category: string;
  amenities: string[];
  description: string;
  images: string[];
  host: { name: string; since: string; superhost: boolean; responseRate: number };
  rules: string[];
  status: ListingStatus;
  submitted: string;
  bookedDates: string[];
};

export const amenityList = [
  "Wi-Fi",
  "Parking",
  "Kitchen",
  "Air conditioning",
  "TV",
  "Pool",
  "Washer",
  "Workspace",
  "Fireplace",
  "Pets allowed",
  "Breakfast",
  "Hot tub",
];

export const categories = [
  "Beachfront",
  "Countryside",
  "Cabins",
  "City lofts",
  "Islands",
  "Design homes",
];

const baseRules = [
  "Check-in after 3:00 PM",
  "Check-out before 11:00 AM",
  "No parties or events",
  "No smoking indoors",
];

function days(start: string, count: number) {
  const out: string[] = [];
  const d = new Date(start);
  for (let i = 0; i < count; i++) {
    out.push(new Date(d.getTime() + i * 86400000).toISOString().slice(0, 10));
  }
  return out;
}

export const properties: Property[] = [
  {
    id: "olive-ridge",
    title: "Olive Ridge — Cliffside Villa with Infinity Pool",
    city: "Paros",
    country: "Greece",
    neighbourhood: "Naoussa Bay",
    price: 340,
    rating: 4.94,
    reviews: 128,
    guests: 8,
    bedrooms: 4,
    beds: 5,
    baths: 3,
    category: "Islands",
    amenities: ["Wi-Fi", "Pool", "Kitchen", "Air conditioning", "Parking", "TV"],
    description:
      "Perched above Naoussa Bay, Olive Ridge pairs warm timber interiors with a 14-metre infinity pool that meets the horizon at sunset. Mornings start with coffee under the olive tree; evenings end on the long teak table with the whole family.",
    images: [hero, p6, p4, p1],
    host: { name: "Elena Marinos", since: "2019", superhost: true, responseRate: 98 },
    rules: baseRules,
    status: "approved",
    submitted: "2024-03-11",
    bookedDates: days("2026-09-04", 6),
  },
  {
    id: "north-loft",
    title: "North Loft — Bright Oak Apartment in the Old Town",
    city: "Copenhagen",
    country: "Denmark",
    neighbourhood: "Nyhavn",
    price: 165,
    rating: 4.87,
    reviews: 214,
    guests: 4,
    bedrooms: 2,
    beds: 2,
    baths: 1,
    category: "City lofts",
    amenities: ["Wi-Fi", "Kitchen", "Washer", "Workspace", "TV"],
    description:
      "A calm two-bedroom loft two streets from the harbour. Herringbone oak floors, tall windows and a proper desk for anyone mixing a few work days into the trip.",
    images: [p1, p7, p5],
    host: { name: "Mikkel Sørensen", since: "2021", superhost: false, responseRate: 92 },
    rules: baseRules,
    status: "approved",
    submitted: "2024-06-02",
    bookedDates: days("2026-08-24", 4),
  },
  {
    id: "casa-fiora",
    title: "Casa Fiora — Restored Stone Farmhouse",
    city: "Val d'Orcia",
    country: "Italy",
    neighbourhood: "Pienza",
    price: 220,
    rating: 4.91,
    reviews: 96,
    guests: 6,
    bedrooms: 3,
    beds: 4,
    baths: 2,
    category: "Countryside",
    amenities: ["Wi-Fi", "Pool", "Kitchen", "Parking", "Fireplace", "Breakfast"],
    description:
      "Seventeenth-century stone, cypress avenue, and a kitchen built for long lunches. Ten minutes from Pienza, an hour from Siena, and quiet enough to hear the wind in the wheat.",
    images: [p2, p7, p4],
    host: { name: "Giulia Ferrari", since: "2018", superhost: true, responseRate: 100 },
    rules: [...baseRules, "Pets welcome with prior notice"],
    status: "approved",
    submitted: "2024-01-19",
    bookedDates: days("2026-09-15", 5),
  },
  {
    id: "pine-hollow",
    title: "Pine Hollow — Glass Cabin in the Forest",
    city: "Åre",
    country: "Sweden",
    neighbourhood: "Björnänge",
    price: 275,
    rating: 4.96,
    reviews: 74,
    guests: 5,
    bedrooms: 2,
    beds: 3,
    baths: 2,
    category: "Cabins",
    amenities: ["Wi-Fi", "Hot tub", "Kitchen", "Fireplace", "Parking", "Pets allowed"],
    description:
      "Floor-to-ceiling glass facing a wall of pines, a wood stove that heats the whole cabin, and a cedar hot tub under the northern sky.",
    images: [p3, p7, p1],
    host: { name: "Anders Lind", since: "2020", superhost: true, responseRate: 95 },
    rules: baseRules,
    status: "approved",
    submitted: "2024-08-07",
    bookedDates: days("2026-08-20", 3),
  },
  {
    id: "salt-house",
    title: "Salt House — Beachfront Home with Open Terrace",
    city: "Comporta",
    country: "Portugal",
    neighbourhood: "Carvalhal",
    price: 295,
    rating: 4.82,
    reviews: 151,
    guests: 7,
    bedrooms: 3,
    beds: 4,
    baths: 3,
    category: "Beachfront",
    amenities: ["Wi-Fi", "Kitchen", "Air conditioning", "Parking", "TV", "Washer"],
    description:
      "Two minutes of soft sand between the terrace and the Atlantic. Whitewashed walls, rattan everywhere, and outdoor showers for coming back from the beach.",
    images: [p4, p6, p2],
    host: { name: "Rui Almeida", since: "2022", superhost: false, responseRate: 88 },
    rules: baseRules,
    status: "approved",
    submitted: "2025-02-14",
    bookedDates: days("2026-09-01", 2),
  },
  {
    id: "skyline-nine",
    title: "Skyline Nine — Penthouse Terrace above the River",
    city: "Lisbon",
    country: "Portugal",
    neighbourhood: "Príncipe Real",
    price: 410,
    rating: 4.78,
    reviews: 63,
    guests: 4,
    bedrooms: 2,
    beds: 2,
    baths: 2,
    category: "Design homes",
    amenities: ["Wi-Fi", "Kitchen", "Air conditioning", "TV", "Workspace", "Washer"],
    description:
      "A ninth-floor apartment with a wraparound terrace, a fire bowl, and the whole city glittering below after dark.",
    images: [p5, p1, p4],
    host: { name: "Sofia Costa", since: "2023", superhost: false, responseRate: 90 },
    rules: baseRules,
    status: "pending",
    submitted: "2026-08-02",
    bookedDates: [],
  },
  {
    id: "barn-eleven",
    title: "Barn Eleven — Converted Hay Barn with Beams",
    city: "Cotswolds",
    country: "United Kingdom",
    neighbourhood: "Stow-on-the-Wold",
    price: 190,
    rating: 4.89,
    reviews: 108,
    guests: 6,
    bedrooms: 3,
    beds: 3,
    baths: 2,
    category: "Countryside",
    amenities: ["Wi-Fi", "Kitchen", "Fireplace", "Parking", "Pets allowed", "Washer"],
    description:
      "Original oak trusses, exposed brick, and wool blankets on every bed. Village pub is a six-minute walk across the field.",
    images: [p7, p2, p3],
    host: { name: "Harriet Doyle", since: "2017", superhost: true, responseRate: 97 },
    rules: baseRules,
    status: "approved",
    submitted: "2023-11-28",
    bookedDates: days("2026-08-27", 4),
  },
  {
    id: "cala-blanca",
    title: "Cala Blanca — Village House with Blue Shutters",
    city: "Menorca",
    country: "Spain",
    neighbourhood: "Binibeca",
    price: 145,
    rating: 4.71,
    reviews: 87,
    guests: 4,
    bedrooms: 2,
    beds: 3,
    baths: 1,
    category: "Islands",
    amenities: ["Wi-Fi", "Kitchen", "Air conditioning", "TV"],
    description:
      "A whitewashed fisherman's house on a quiet lane, bougainvillea over the door, and a cove you can swim in before breakfast.",
    images: [p6, p4, p2],
    host: { name: "Rui Almeida", since: "2022", superhost: false, responseRate: 88 },
    rules: baseRules,
    status: "draft",
    submitted: "2026-08-12",
    bookedDates: [],
  },
];

export const destinations = [
  { city: "Paros", country: "Greece", stays: 148, image: hero },
  { city: "Val d'Orcia", country: "Italy", stays: 96, image: p2 },
  { city: "Copenhagen", country: "Denmark", stays: 212, image: p1 },
  { city: "Comporta", country: "Portugal", stays: 74, image: p4 },
];

export type Booking = {
  id: string;
  propertyId: string;
  guest: string;
  guestEmail: string;
  checkIn: string;
  checkOut: string;
  guests: number;
  total: number;
  status: BookingStatus;
  createdAt: string;
};

export const bookings: Booking[] = [
  {
    id: "HV-4821",
    propertyId: "olive-ridge",
    guest: "Nadia Rahman",
    guestEmail: "nadia.rahman@mail.com",
    checkIn: "2026-09-04",
    checkOut: "2026-09-10",
    guests: 6,
    total: 2196,
    status: "approved",
    createdAt: "2026-07-28",
  },
  {
    id: "HV-4832",
    propertyId: "pine-hollow",
    guest: "Tom Bergman",
    guestEmail: "t.bergman@mail.com",
    checkIn: "2026-08-20",
    checkOut: "2026-08-23",
    guests: 4,
    total: 897,
    status: "pending",
    createdAt: "2026-08-14",
  },
  {
    id: "HV-4790",
    propertyId: "north-loft",
    guest: "Amira Haddad",
    guestEmail: "amira.h@mail.com",
    checkIn: "2026-06-11",
    checkOut: "2026-06-15",
    guests: 2,
    total: 712,
    status: "completed",
    createdAt: "2026-05-30",
  },
  {
    id: "HV-4755",
    propertyId: "casa-fiora",
    guest: "Lucas Meyer",
    guestEmail: "lucas.meyer@mail.com",
    checkIn: "2026-05-02",
    checkOut: "2026-05-07",
    guests: 5,
    total: 1188,
    status: "completed",
    createdAt: "2026-04-15",
  },
  {
    id: "HV-4844",
    propertyId: "salt-house",
    guest: "Yara Fahmy",
    guestEmail: "yara.fahmy@mail.com",
    checkIn: "2026-09-01",
    checkOut: "2026-09-03",
    guests: 4,
    total: 654,
    status: "cancelled",
    createdAt: "2026-08-01",
  },
  {
    id: "HV-4851",
    propertyId: "barn-eleven",
    guest: "Chloe Deveraux",
    guestEmail: "chloe.d@mail.com",
    checkIn: "2026-08-27",
    checkOut: "2026-08-31",
    guests: 6,
    total: 836,
    status: "pending",
    createdAt: "2026-08-16",
  },
  {
    id: "HV-4712",
    propertyId: "olive-ridge",
    guest: "Peter Novak",
    guestEmail: "p.novak@mail.com",
    checkIn: "2026-04-09",
    checkOut: "2026-04-13",
    guests: 8,
    total: 1462,
    status: "rejected",
    createdAt: "2026-03-22",
  },
];

export type Review = {
  id: string;
  propertyId: string;
  author: string;
  date: string;
  rating: number;
  body: string;
  hostResponse?: string;
};

export const reviews: Review[] = [
  {
    id: "r1",
    propertyId: "olive-ridge",
    author: "Amira Haddad",
    date: "July 2026",
    rating: 5,
    body: "The photos undersell the view. Elena left a bottle of local wine and a hand-drawn map of the swimming coves. The pool is genuinely as good as it looks.",
    hostResponse:
      "Thank you Amira — you left the house spotless. Come back in spring when the olives are flowering.",
  },
  {
    id: "r2",
    propertyId: "olive-ridge",
    author: "Lucas Meyer",
    date: "June 2026",
    rating: 5,
    body: "Four adults and four kids and nobody felt crowded. The kitchen is well equipped and the drive down to Naoussa is only ten minutes.",
  },
  {
    id: "r3",
    propertyId: "olive-ridge",
    author: "Sara Bensalem",
    date: "May 2026",
    rating: 4,
    body: "Beautiful house and a very responsive host. The road up is steep — take a proper car, not a scooter.",
  },
  {
    id: "r4",
    propertyId: "north-loft",
    author: "Tom Bergman",
    date: "June 2026",
    rating: 5,
    body: "Perfect base for four days in the city. Quiet street, excellent bed, and the desk made a work morning painless.",
  },
  {
    id: "r5",
    propertyId: "casa-fiora",
    author: "Nadia Rahman",
    date: "May 2026",
    rating: 5,
    body: "Giulia's breakfast baskets alone are worth the booking. We spent every evening on the terrace watching the light go over the valley.",
    hostResponse: "It was a pleasure hosting you. The terrace misses you already!",
  },
];

export type PlatformUser = {
  id: string;
  name: string;
  email: string;
  role: "Guest" | "Host" | "Admin";
  status: "Active" | "Suspended" | "Pending";
  joined: string;
  bookings: number;
};

export const users: PlatformUser[] = [
  { id: "u1", name: "Elena Marinos", email: "elena.marinos@havenly.co", role: "Host", status: "Active", joined: "2019-04-12", bookings: 214 },
  { id: "u2", name: "Nadia Rahman", email: "nadia.rahman@mail.com", role: "Guest", status: "Active", joined: "2024-02-03", bookings: 9 },
  { id: "u3", name: "Giulia Ferrari", email: "giulia@casafiora.it", role: "Host", status: "Active", joined: "2018-09-21", bookings: 168 },
  { id: "u4", name: "Tom Bergman", email: "t.bergman@mail.com", role: "Guest", status: "Active", joined: "2025-06-18", bookings: 3 },
  { id: "u5", name: "Rui Almeida", email: "rui.almeida@mail.com", role: "Host", status: "Suspended", joined: "2022-11-05", bookings: 47 },
  { id: "u6", name: "Yara Fahmy", email: "yara.fahmy@mail.com", role: "Guest", status: "Active", joined: "2026-01-09", bookings: 2 },
  { id: "u7", name: "Mikkel Sørensen", email: "mikkel@northloft.dk", role: "Host", status: "Active", joined: "2021-03-30", bookings: 121 },
  { id: "u8", name: "Chloe Deveraux", email: "chloe.d@mail.com", role: "Guest", status: "Pending", joined: "2026-08-10", bookings: 0 },
  { id: "u9", name: "Omar Khalil", email: "omar.khalil@havenly.co", role: "Admin", status: "Active", joined: "2023-05-02", bookings: 0 },
];

export const activity = [
  { id: "a1", type: "listing", text: "Sofia Costa submitted “Skyline Nine” for review", time: "12 minutes ago" },
  { id: "a2", type: "booking", text: "Chloe Deveraux requested 4 nights at “Barn Eleven”", time: "48 minutes ago" },
  { id: "a3", type: "user", text: "Chloe Deveraux created a guest account", time: "1 hour ago" },
  { id: "a4", type: "review", text: "Amira Haddad left a 5-star review on “Olive Ridge”", time: "3 hours ago" },
  { id: "a5", type: "approved", text: "“Pine Hollow” was approved and published", time: "Yesterday" },
  { id: "a6", type: "cancelled", text: "Booking HV-4844 was cancelled by the guest", time: "Yesterday" },
  { id: "a7", type: "booking", text: "Tom Bergman requested 3 nights at “Pine Hollow”", time: "2 days ago" },
  { id: "a8", type: "user", text: "Rui Almeida was suspended after 3 policy reports", time: "3 days ago" },
];

export const revenueSeries = [
  { month: "Mar", revenue: 3120, bookings: 8 },
  { month: "Apr", revenue: 4480, bookings: 11 },
  { month: "May", revenue: 5260, bookings: 13 },
  { month: "Jun", revenue: 4890, bookings: 12 },
  { month: "Jul", revenue: 7320, bookings: 18 },
  { month: "Aug", revenue: 8140, bookings: 21 },
];

export const platformSeries = [
  { month: "Mar", bookings: 214, signups: 96 },
  { month: "Apr", bookings: 268, signups: 121 },
  { month: "May", bookings: 341, signups: 148 },
  { month: "Jun", bookings: 392, signups: 133 },
  { month: "Jul", bookings: 486, signups: 187 },
  { month: "Aug", bookings: 528, signups: 204 },
];

export function getProperty(id: string) {
  return properties.find((p) => p.id === id);
}

export function nightsBetween(a?: string, b?: string) {
  if (!a || !b) return 0;
  const diff = (new Date(b).getTime() - new Date(a).getTime()) / 86400000;
  return diff > 0 ? Math.round(diff) : 0;
}

export function formatMoney(n: number) {
  return `$${n.toLocaleString("en-US")}`;
}

export function formatDate(d?: string) {
  if (!d) return "—";
  return new Date(d).toLocaleDateString("en-US", {
    month: "short",
    day: "numeric",
    year: "numeric",
  });
}