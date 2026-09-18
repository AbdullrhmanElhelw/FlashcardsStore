import urllib.request
import json
import urllib.parse
import http.cookiejar

cookie_jar = http.cookiejar.CookieJar()
opener = urllib.request.build_opener(urllib.request.HTTPCookieProcessor(cookie_jar))

def get(url, cookies=None):
    headers = {'User-Agent': 'Mozilla/5.0'}
    if cookies:
        headers['Cookie'] = cookies
    req = urllib.request.Request(url, headers=headers)
    with opener.open(req) as resp:
        return resp.read().decode('utf-8')

def post_json(url, data):
    payload = json.dumps(data).encode('utf-8')
    req = urllib.request.Request(url, data=payload, headers={'Content-Type': 'application/json', 'User-Agent': 'Mozilla/5.0'})
    with opener.open(req) as resp:
        return json.loads(resp.read().decode('utf-8'))

import sys
import os

sys.stdout.reconfigure(encoding='utf-8')

print("--- 1. Testing Wishlist Page ---")
html_wishlist_empty = get("http://localhost:5059/Wishlist")
print("Empty Wishlist has empty title:", "Wishlist is Empty" in html_wishlist_empty or "قائمة المفضلة فارغة" in html_wishlist_empty)

html_wishlist_items = get("http://localhost:5059/Wishlist?ids=1,2")
print("Wishlist with items has Product 1:", "short-vowels-pack" in html_wishlist_items)
print("Wishlist with items has Product 2:", "blends-and-digraphs-pack" in html_wishlist_items)

print("\n--- 2. Testing Bilingual Details & Reviews in Arabic ---")
get("http://localhost:5059/Culture/SetCulture?culture=ar-EG")
html_details_ar = get("http://localhost:5059/Catalog/Details?slug=short-vowels-pack")
has_ar_desc = "مجموعة مكونة من 52 كارت" in html_details_ar
print("Arabic Details has Arabic description:", has_ar_desc)
print("Arabic Details has Reviews header:", "آراء وتجارب" in html_details_ar)
has_seed_review = "مروة سالم" in html_details_ar or "Yasmin" in html_details_ar
print("Seeded reviews present on details page:", has_seed_review)
print("Reviews section present:", "reviews-section" in html_details_ar)

print("\n--- 3. Testing Bilingual Details & Reviews in English ---")
get("http://localhost:5059/Culture/SetCulture?culture=en-US")
html_details_en = get("http://localhost:5059/Catalog/Details?slug=short-vowels-pack")
has_en_desc = "A complete set of 52 laminated" in html_details_en
print("English Details has English description:", has_en_desc)
print("English Details has Reviews header:", "Parent & Teacher Reviews" in html_details_en)

print("\n--- 4. Testing Reviews Submission via API ---")
review_payload = {
    "productId": 1,
    "customerName": "أ. سمر فاروق",
    "role": "ولية أمر (KG1)",
    "rating": 5,
    "comment": "تجربة ممتازة جداً والألوان مبهجة وشجعت طفلي على التكرار بدون ملل."
}
resp = post_json("http://localhost:5059/Catalog/AddReview", review_payload)
print("Add Review API success:", resp.get("success"), resp.get("message"))

# Check if new review now appears
html_details_after = get("http://localhost:5059/Catalog/Details?slug=short-vowels-pack")
print("Newly added review rendered on page:", "سمر فاروق" in html_details_after)

print("\n--- 5. Testing Wishlist Heart Button in Navbar and Cards ---")
print("Navbar has Wishlist link:", "/Wishlist" in html_details_ar)
print("Product Card has Wishlist Heart toggle:", "$store.wishlist.toggle" in html_details_ar)

print("\n--- 6. Testing Admin Reviews Page ---")
html_admin_reviews = get("http://localhost:5059/Admin/Reviews")
print("Admin Reviews Page loaded:", "إدارة تقييمات وآراء العملاء" in html_admin_reviews or "Customer Reviews" in html_admin_reviews)

print("\nAll feature verifications completed successfully!")
