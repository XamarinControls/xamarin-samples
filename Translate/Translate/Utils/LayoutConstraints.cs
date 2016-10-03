using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Foundation;
using UIKit;

namespace Translate.Utils
{
    public static class LayoutConstraints
    {
        /// <summary>
        /// Add a single constraint for a View
        /// </summary>
        /// <param name="parent">The View with the control</param>
        /// <param name="format">The visual format string. For e.g. '|-[label1(100)]-|'.</param>
        /// <param name="viewName">The name  of the view used in <param name="format"/>.</param>
        /// <param name="view">The view associated with <paramref name="viewName"/></param>
        /// <param name="setTranslatesAutoresizingMaskIntoConstraintsToFalse"></param>
        public static void AddConstraint(UIView parent, string format, string viewName, UIView view, bool setTranslatesAutoresizingMaskIntoConstraintsToFalse = true)
        {
            AddConstraints(parent, new[] { format }, new Dictionary<string, UIView> { { viewName, view } }, setTranslatesAutoresizingMaskIntoConstraintsToFalse);
        }

        /// <summary>
        /// Add a single constraint for a View
        /// </summary>
        /// <param name="parent">The View with the control</param>
        /// <param name="format">The visual format string. For e.g. '|-[label1(100)]-|'</param>
        /// <param name="dict">A dictionary with the used controls and there variable names</param>
        /// <param name="setTranslatesAutoresizingMaskIntoConstraintsToFalse"></param>
        public static void AddConstraints(UIView parent, string format, IDictionary<string, UIView> dict, bool setTranslatesAutoresizingMaskIntoConstraintsToFalse = true)
        {
            AddConstraints(parent, new[] { format }, dict, setTranslatesAutoresizingMaskIntoConstraintsToFalse);
        }

        /// <summary>
        /// Adds constraints for a View
        /// </summary>
        /// <param name="parent">The View with the controls</param>
        /// <param name="formats">The visual format strings. For e.g. '|-[label1(100)]-|'</param>
        /// <param name="dict">A dictionary with the used controls and there variable names</param>
        /// <param name="setTranslatesAutoresizingMaskIntoConstraintsToFalse"></param>
        public static NSLayoutConstraint[] AddConstraints(UIView parent, IEnumerable<string> formats, IDictionary<string, UIView> dict,
                                          bool setTranslatesAutoresizingMaskIntoConstraintsToFalse = true)
        {
            try
            {
                if (setTranslatesAutoresizingMaskIntoConstraintsToFalse)
                {
                    foreach (var view in dict.Values)
                    {
                        view.TranslatesAutoresizingMaskIntoConstraints = false;
                    }
                }

                var constraints = BuildConstraints(formats, dict);
                parent.AddConstraints(constraints);
                return constraints;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Adds constraints for setting the <paramref name="child">childs</paramref> width, height, top, left equal to the
        /// respective values of the <paramref name="parent">parent view</paramref>.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <param name="setTranslatesAutoresizingMaskIntoConstraintsToFalse"></param>
        public static void AddFullStretchConstraints(UIView parent, UIView child,
                                                     bool setTranslatesAutoresizingMaskIntoConstraintsToFalse = true)
        {
            AddEqualityConstraint(parent, child, parent, NSLayoutAttribute.Width, setTranslatesAutoresizingMaskIntoConstraintsToFalse);
            AddEqualityConstraint(parent, child, parent, NSLayoutAttribute.Height, setTranslatesAutoresizingMaskIntoConstraintsToFalse);
            AddEqualityConstraint(parent, child, parent, NSLayoutAttribute.Top, setTranslatesAutoresizingMaskIntoConstraintsToFalse);
            AddEqualityConstraint(parent, child, parent, NSLayoutAttribute.Left, setTranslatesAutoresizingMaskIntoConstraintsToFalse);
        }

        /// <summary>
        /// Adds constraints for setting the <paramref name="child">childs</paramref> top, bottom, left, right equal to the
        /// respective values of the <paramref name="parent">parent view</paramref>.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <param name="setTranslatesAutoresizingMaskIntoConstraintsToFalse"></param>
        public static void AddAllEdgeEqualityConstraints(UIView parent, UIView child,
                                                         bool setTranslatesAutoresizingMaskIntoConstraintsToFalse = true)
        {
            AddEqualityConstraint(parent, child, parent, NSLayoutAttribute.Top, setTranslatesAutoresizingMaskIntoConstraintsToFalse);
            AddEqualityConstraint(parent, child, parent, NSLayoutAttribute.Bottom, setTranslatesAutoresizingMaskIntoConstraintsToFalse);
            AddEqualityConstraint(parent, child, parent, NSLayoutAttribute.Left, setTranslatesAutoresizingMaskIntoConstraintsToFalse);
            AddEqualityConstraint(parent, child, parent, NSLayoutAttribute.Right, setTranslatesAutoresizingMaskIntoConstraintsToFalse);
        }

        /// <summary>
        /// Adds constraints for setting the <paramref name="child">childs</paramref> top, bottom, left, right equal to the
        /// respective values of the <paramref name="relativeToView">relativeToView</paramref>.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <param name="relativeToView"></param>
        /// <param name="setTranslatesAutoresizingMaskIntoConstraintsToFalse"></param>
        public static void AddAllEdgeEqualityConstraints(UIView parent, UIView child, UIView relativeToView,
                                                         bool setTranslatesAutoresizingMaskIntoConstraintsToFalse = true)
        {
            AddEqualityConstraint(parent, child, relativeToView, NSLayoutAttribute.Top, setTranslatesAutoresizingMaskIntoConstraintsToFalse);
            AddEqualityConstraint(parent, child, relativeToView, NSLayoutAttribute.Bottom, setTranslatesAutoresizingMaskIntoConstraintsToFalse);
            AddEqualityConstraint(parent, child, relativeToView, NSLayoutAttribute.Left, setTranslatesAutoresizingMaskIntoConstraintsToFalse);
            AddEqualityConstraint(parent, child, relativeToView, NSLayoutAttribute.Right, setTranslatesAutoresizingMaskIntoConstraintsToFalse);
        }

        public static void AddEqualityConstraints(UIView parent, IEnumerable<UIView> childs, NSLayoutAttribute attribute,
                                                         bool setTranslatesAutoresizingMaskIntoConstraintsToFalse = true)
        {
            var first = childs.FirstOrDefault();
            foreach (var child in childs.Skip(1))
            {
                AddEqualityConstraint(parent, first, child, attribute, setTranslatesAutoresizingMaskIntoConstraintsToFalse);
            }
        }

        /// <summary>
        /// Adds constraints for setting the <paramref name="child">childs</paramref> width, height equal to the
        /// respective values of the <paramref name="parent">parent view</paramref>.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <param name="setTranslatesAutoresizingMaskIntoConstraintsToFalse"></param>
        public static void AddEqualSizeConstraints(UIView parent, UIView child,
                                                   bool setTranslatesAutoresizingMaskIntoConstraintsToFalse = true)
        {
            AddEqualityConstraint(parent, child, parent, NSLayoutAttribute.Width, setTranslatesAutoresizingMaskIntoConstraintsToFalse);
            AddEqualityConstraint(parent, child, parent, NSLayoutAttribute.Height, setTranslatesAutoresizingMaskIntoConstraintsToFalse);
        }

        /// <summary>
        /// Adds a constraint that sets the height of <paramref name="child"/> to <paramref name="height"/>.
        /// </summary>
        /// <param name="parent">The View within the control</param>
        /// <param name="child"></param>
        /// <param name="height"></param>
        /// <param name="setTranslatesAutoresizingMaskIntoConstraintsToFalse"></param>
        public static NSLayoutConstraint AddHeightConstraint(UIView parent, UIView child, float height, bool setTranslatesAutoresizingMaskIntoConstraintsToFalse = true)
        {
            return AddConstantConstraint(parent, child, NSLayoutAttribute.Height, height,
                                         setTranslatesAutoresizingMaskIntoConstraintsToFalse);
        }

        /// <summary>
        /// Adds a constraint that sets the width of <paramref name="child"/> to <paramref name="width"/>.
        /// </summary>
        /// <param name="parent">The View within the control</param>
        /// <param name="child"></param>
        /// <param name="width"></param>
        /// <param name="setTranslatesAutoresizingMaskIntoConstraintsToFalse"></param>
        public static NSLayoutConstraint AddWidthConstraint(UIView parent, UIView child, float width, bool setTranslatesAutoresizingMaskIntoConstraintsToFalse = true)
        {
            return AddConstantConstraint(parent, child, NSLayoutAttribute.Width, width,
                                         setTranslatesAutoresizingMaskIntoConstraintsToFalse);
        }


        /// <summary>
        /// Adds a constraint that sets the <paramref name="attribute"/> of <paramref name="child"/> to <paramref name="constant"/>.
        /// </summary>
        /// <param name="parent">The View within the control</param>
        /// <param name="child"></param>
        /// <param name="constant">constant value</param>
        /// <param name="setTranslatesAutoresizingMaskIntoConstraintsToFalse"></param>
        /// <param name="attribute">Attribute to set.</param>
        public static NSLayoutConstraint AddConstantConstraint(UIView parent, UIView child, NSLayoutAttribute attribute, float constant, bool setTranslatesAutoresizingMaskIntoConstraintsToFalse = true)
        {
            if (setTranslatesAutoresizingMaskIntoConstraintsToFalse)
            {
                child.TranslatesAutoresizingMaskIntoConstraints = false;
            }

            var constraint = NSLayoutConstraint.Create(child, attribute, NSLayoutRelation.Equal, null,
                                                       NSLayoutAttribute.NoAttribute, 1, constant);

            parent.AddConstraint(constraint);

            return constraint;
        }

        /// <summary>
        /// Adds a constraint for width
        /// </summary>
        /// <param name="parent">The View within the control</param>
        /// <param name="child">The control to center</param>
        /// <param name="multiplier"></param>
        /// <param name="setTranslatesAutoresizingMaskIntoConstraintsToFalse"></param>
        public static void AddPercentWidthConstraint(UIView parent, UIView child, float multiplier, bool setTranslatesAutoresizingMaskIntoConstraintsToFalse = true)
        {
            if (setTranslatesAutoresizingMaskIntoConstraintsToFalse)
            {
                child.TranslatesAutoresizingMaskIntoConstraints = false;
            }

            var constraint = NSLayoutConstraint.Create(child, NSLayoutAttribute.Width, NSLayoutRelation.Equal, parent,
                                                       NSLayoutAttribute.Width, multiplier, 0);

            parent.AddConstraint(constraint);
        }

        /// <summary>
        /// Adds a constraint for horizontal center layout
        /// </summary>
        /// <param name="parent">The View within the control</param>
        /// <param name="child">The control to center</param>
        /// <param name="multiplier"></param>
        /// <param name="setTranslatesAutoresizingMaskIntoConstraintsToFalse"></param>
        public static void AddPercentHeightConstraint(UIView parent, UIView child, float multiplier, bool setTranslatesAutoresizingMaskIntoConstraintsToFalse = true)
        {
            if (setTranslatesAutoresizingMaskIntoConstraintsToFalse)
            {
                child.TranslatesAutoresizingMaskIntoConstraints = false;
            }

            var constraint = NSLayoutConstraint.Create(child, NSLayoutAttribute.Height, NSLayoutRelation.Equal, parent,
                                                       NSLayoutAttribute.Height, multiplier, 0);

            parent.AddConstraint(constraint);
        }

        /// <summary>
        /// Adds a constraint wich sets the <paramref name="attribute"/> of the <paramref name="child"/> 
        /// equal to the <paramref name="attribute"/> of the <paramref name="relativeToView"/>.
        /// </summary>
        /// <param name="parent">The View within the control</param>
        /// <param name="child"></param>
        /// <param name="attribute"></param>
        /// <param name="setTranslatesAutoresizingMaskIntoConstraintsToFalse"></param>
        /// <param name="relativeToView"></param>
        public static NSLayoutConstraint AddEqualityConstraint(UIView parent, UIView child, UIView relativeToView, NSLayoutAttribute attribute,
                                                 bool setTranslatesAutoresizingMaskIntoConstraintsToFalse = true)
        {
            return AddEqualityConstraint(parent, child, relativeToView, attribute, attribute,
                                         setTranslatesAutoresizingMaskIntoConstraintsToFalse);
        }

        /// <summary>
        /// Adds a constraint wich sets the <paramref name="childAttribute">attribute</paramref> of the <paramref name="child"/> 
        /// equal to the <paramref name="relativeToAttribute">attribute</paramref> of the <paramref name="relativeToView"/>.
        /// </summary>
        /// <param name="parent">The View within the control</param>
        /// <param name="child"></param>
        /// <param name="relativeToAttribute"></param>
        /// <param name="setTranslatesAutoresizingMaskIntoConstraintsToFalse"></param>
        /// <param name="relativeToView"></param>
        /// <param name="childAttribute"></param>
        public static NSLayoutConstraint AddEqualityConstraint(UIView parent, UIView child, UIView relativeToView, NSLayoutAttribute childAttribute, NSLayoutAttribute relativeToAttribute, bool setTranslatesAutoresizingMaskIntoConstraintsToFalse = true)
        {
            return AddRelativeConstraint(parent, child, relativeToView, childAttribute, relativeToAttribute, 1, 0, setTranslatesAutoresizingMaskIntoConstraintsToFalse);
        }

        public static NSLayoutConstraint AddRelativeConstraint(UIView parent, UIView child, UIView relativeToView, NSLayoutAttribute childAttribute, NSLayoutAttribute relativeToAttribute, float multiplier = 1, float constant = 0.0f, bool setTranslatesAutoresizingMaskIntoConstraintsToFalse = true)
        {
            if (setTranslatesAutoresizingMaskIntoConstraintsToFalse)
            {
                child.TranslatesAutoresizingMaskIntoConstraints = false;
            }

            var constraint = NSLayoutConstraint.Create(child, childAttribute, NSLayoutRelation.Equal, relativeToView,
                                                       relativeToAttribute, multiplier, constant);

            parent.AddConstraint(constraint);

            return constraint;
        }

        /// <summary>
        /// Adds a constraint for horizental and vertical center layout
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <param name="setTranslatesAutoresizingMaskIntoConstraintsToFalse"></param>
        public static NSLayoutConstraint[] AddFullCenterConstraint(UIView parent, UIView child, bool setTranslatesAutoresizingMaskIntoConstraintsToFalse = true)
        {
            if (setTranslatesAutoresizingMaskIntoConstraintsToFalse)
            {
                child.TranslatesAutoresizingMaskIntoConstraints = false;
            }

            var constraintX = NSLayoutConstraint.Create(child, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, parent,
                                                       NSLayoutAttribute.CenterX, 1, 0);
            var constraintY = NSLayoutConstraint.Create(child, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, parent,
                                                       NSLayoutAttribute.CenterY, 1, 0);

            var nsLayoutConstraints = new[] { constraintX, constraintY };
            parent.AddConstraints(nsLayoutConstraints);
            return nsLayoutConstraints;
        }

        /// <summary>
        /// Adds a constraint for horizental center layout
        /// </summary>
        /// <param name="parent">The View within the control</param>
        /// <param name="child">The control to center</param>
        /// <param name="setTranslatesAutoresizingMaskIntoConstraintsToFalse"></param>
        public static NSLayoutConstraint AddHorizontalCenterConstraint(UIView parent, UIView child, bool setTranslatesAutoresizingMaskIntoConstraintsToFalse = true)
        {
            if (setTranslatesAutoresizingMaskIntoConstraintsToFalse)
            {
                child.TranslatesAutoresizingMaskIntoConstraints = false;
            }

            var constraint = NSLayoutConstraint.Create(child, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, parent,
                                                       NSLayoutAttribute.CenterX, 1, 0);

            parent.AddConstraint(constraint);
            return constraint;
        }

        /// <summary>
        /// Adds a constraint for vertical center layout
        /// </summary>
        /// <param name="parent">The View within the control</param>
        /// <param name="child">The control to center</param>
        /// <param name="setTranslatesAutoresizingMaskIntoConstraintsToFalse"></param>
        public static NSLayoutConstraint AddVerticalCenterConstraint(UIView parent, UIView child, bool setTranslatesAutoresizingMaskIntoConstraintsToFalse = true)
        {
            return AddEqualityConstraint(parent, child, parent, NSLayoutAttribute.CenterY, setTranslatesAutoresizingMaskIntoConstraintsToFalse);
        }


        /// <summary>
        /// Adds a constraint to set on top of parent view
        /// </summary>
        /// <param name="parent">The View within the control</param>
        /// <param name="child">The control to set to top</param>
        /// <param name="setTranslatesAutoresizingMaskIntoConstraintsToFalse"></param>
        public static NSLayoutConstraint AddTopConstraint(UIView parent, UIView child, bool setTranslatesAutoresizingMaskIntoConstraintsToFalse = true)
        {
            return AddEqualityConstraint(parent, child, parent, NSLayoutAttribute.Top, setTranslatesAutoresizingMaskIntoConstraintsToFalse);
        }

        /// <summary>
        /// Adds a constraint to set the base line of <param name="child"/> to top + <param name="baseLine"/> of parent view
        /// </summary>
        /// <param name="parent">The View within the control</param>
        /// <param name="child">The control to set to top</param>
        /// <param name="baseLine">Offest</param>
        /// <param name="setTranslatesAutoresizingMaskIntoConstraintsToFalse"></param>
        public static NSLayoutConstraint AddBaseLineConstraint(UIView parent, UIView child, int baseLine, bool setTranslatesAutoresizingMaskIntoConstraintsToFalse = true)
        {
            if (setTranslatesAutoresizingMaskIntoConstraintsToFalse)
            {
                child.TranslatesAutoresizingMaskIntoConstraints = false;
            }

            var constraint = NSLayoutConstraint.Create(child, NSLayoutAttribute.Baseline, NSLayoutRelation.Equal, parent, NSLayoutAttribute.Top, 1, baseLine);

            parent.AddConstraint(constraint);
            return constraint;
        }

        /// <summary>
        /// Adds a constraint to set the base line of <param name="child"/> to <paramref name="relativeToAttribute"/> + <param name="baseLine"/> of <paramref name="relativeToView"/>.
        /// </summary>
        /// <param name="parent">The View within the control</param>
        /// <param name="child">The control to set to top</param>
        /// <param name="relativeToAttribute"></param>
        /// <param name="baseLine">Offest</param>
        /// <param name="setTranslatesAutoresizingMaskIntoConstraintsToFalse"></param>
        /// <param name="relativeToView"></param>
        public static NSLayoutConstraint AddBaseLineRelativeConstraint(UIView parent, UIView child, UIView relativeToView, NSLayoutAttribute relativeToAttribute, int baseLine, bool setTranslatesAutoresizingMaskIntoConstraintsToFalse = true)
        {
            if (setTranslatesAutoresizingMaskIntoConstraintsToFalse)
            {
                child.TranslatesAutoresizingMaskIntoConstraints = false;
            }

            var constraint = NSLayoutConstraint.Create(child, NSLayoutAttribute.Baseline, NSLayoutRelation.Equal, relativeToView,
                                      relativeToAttribute, 1, baseLine);

            parent.AddConstraint(constraint);
            return constraint;
        }

        private static NSLayoutConstraint[] BuildConstraints(IEnumerable<string> formats, IDictionary<string, UIView> dict)
        {
            var nsdict = NSDictionary.FromObjectsAndKeys(dict.Values.Cast<object>().ToArray(),
                                                         dict.Keys.Cast<object>().ToArray());

            var constraints = from format in formats
                              let f = format.Replace("\r", "").Replace("\n", "").Replace(" ", "")
                              select NSLayoutConstraint.FromVisualFormat(f, 0, null, nsdict);

            return constraints.SelectMany(c => c).ToArray();
        }

        public static void PrepareAutoLayoutRecursive(UIView control)
        {
            control.TranslatesAutoresizingMaskIntoConstraints = false;

            foreach (var view in control.Subviews)
            {
                PrepareAutoLayoutRecursive(view);
            }
        }

        /// <summary>
        /// Gets a spacing string in visual format language style.
        /// </summary>
        /// <example>GetSpacing(10); will will return "-(10)-"</example>
        /// <param name="space">Space.</param>
        /// <returns></returns>
        public static string GetSpacing(float space)
        {
            return string.Format("-({0})-", space.ToString(CultureInfo.InvariantCulture));
        }
    }
}